using Infrastructure.Business.Dto;
using Infrastructure.Business.Service;
using Infrastructure.Data.Models;
using Infrastructure.Data.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Business.Managers
{
    public class BookingManager
    {
        private readonly BookingRepository _bookingRepository;
        private readonly EventServiceClient _eventClient;
        private readonly BookingServiceBusSender _serviceBusSender;
        public BookingManager(BookingRepository bookingRepository, EventServiceClient eventClient, BookingServiceBusSender serviceBusSender)
        {
            _bookingRepository = bookingRepository;
            _eventClient = eventClient;
            _serviceBusSender = serviceBusSender;
        }

        public async Task<(bool Success, string? ErrorMessage)> AddBookingWithTicketsAsync(BookingDto dto)
        {
            try
            {
                var eventData = await _eventClient.GetEventByIdAsync(dto.EventId);
                if (eventData == null)
                    return (false, "Event not found");

                foreach (var requestedTicket in dto.Tickets)
                {
                    var actualTicket = eventData.TicketTypes.FirstOrDefault(t => t.Id == requestedTicket.TicketTypeId);
                    if (actualTicket == null)
                        return (false, $"TicketType ID {requestedTicket.TicketTypeId} not found in event.");

                    if (actualTicket.TicketType_ != requestedTicket.TicketType)
                        return (false, $"TicketType name mismatch for ID {requestedTicket.TicketTypeId}");

                    if (actualTicket.Price != requestedTicket.PricePerTicket)
                        return (false, $"Ticket price mismatch for {actualTicket.TicketType_}");

                    if (actualTicket.TicketsLeft < requestedTicket.Quantity)
                        return (false, $"Not enough tickets left for {actualTicket.TicketType_}");
                }
                var evoucherId = GenerateEvoucherId();
                var booking = new BookingEntity
                {
                    InvoiceId = dto.InvoiceId,
                    BookingName = dto.BookingName,
                    CreatedAt = dto.CreatedAt,
                    EventId = dto.EventId,
                    EvoucherId = evoucherId,
                    Tickets = dto.Tickets.Select(t => new BookedTicketEntity
                    {
                        TicketTypeId = t.TicketTypeId,
                        TicketType = t.TicketType,
                        Quantity = t.Quantity,
                        PricePerTicket = t.PricePerTicket
                    }).ToList()

                };

                var result = await _bookingRepository.AddAsync(booking);
                if (result)
                {
                    await _serviceBusSender.SendBookingCreatedAsync(
                        booking.Id,
                        booking.EventId,
                        booking.Tickets.ToList()
                        );
                    return (true, null);
                }
                else
                {
                    return (false, "Could not save booking to database");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in AddBookingWithTicketsAsync: {ex.Message}");
                return (false, "Unexpected error while adding booking");
            }
        }
        private static string GenerateEvoucherId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Range(0, 6).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        public async Task<List<BookingEntity>> GetAllBookingsAsync()
        {
            return (await _bookingRepository.GetAllAsync(includes: b => b.Tickets)).ToList();
        }

        public async Task<(bool Success, string? Message)> UpdateBookingAsync(BookingEntity updatedBooking)
        {
            try
            {
                var success = await _bookingRepository.UpdateAsync(updatedBooking);
                return success
                    ? (true, null)
                    : (false, "Booking not found or update failed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateBookingAsync error: {ex.Message}");
                return (false, "Unexpected error while updating booking");
            }
        }

        public async Task<(bool Success, string? Message)> DeleteBookingAsync(int id)
        {
            try
            {
                var success = await _bookingRepository.DeleteAsync(b => b.Id == id);
                return success
                    ? (true, null)
                    : (false, "Booking not found or could not be deleted");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteBookingAsync error: {ex.Message}");
                return (false, "Unexpected error while deleting booking");
            }
        }

        public async Task<BookingStatsDto> GetBookingStatsAsync()
        {
            var bookings = await _bookingRepository.GetAllWithTicketsAsync();

            var totalBookings = bookings.Count;
            var totalTickets = bookings.Sum(b => b.Tickets.Sum(t => t.Quantity));
            var totalEarnings = bookings.Sum(b => b.Tickets.Sum(t => t.Quantity * t.PricePerTicket));

            return new BookingStatsDto
            {
                TotalBookings = totalBookings,
                TotalTicketsSold = totalTickets,
                TotalEarnings = totalEarnings
            };
        }

        public async Task<BookingChartDto> GetBookingOverviewAsync(string range)
        {
            var allBookings = await _bookingRepository.GetAllAsync();

            var grouped = range.ToLower() switch
            {
                "today" => allBookings
                    .Where(b => b.CreatedAt.Date == DateTime.Today)
                    .GroupBy(b => b.CreatedAt.Hour)
                    .OrderBy(g => g.Key)
                    .Select(g => new { Label = $"{g.Key}:00", Count = g.Count() }),

                "week" => allBookings
                    .Where(b => b.CreatedAt >= DateTime.Today.AddDays(-6))
                    .GroupBy(b => b.CreatedAt.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new { Label = g.Key.ToString("ddd"), Count = g.Count() }),

                "month" => allBookings
                    .Where(b => b.CreatedAt >= DateTime.Today.AddDays(-30))
                    .GroupBy(b => b.CreatedAt.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new { Label = g.Key.ToString("MM-dd"), Count = g.Count() }),

                _ => allBookings
                    .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                    .Select(g => new { Label = $"{g.Key.Month}/{g.Key.Year}", Count = g.Count() })
            };

            return new BookingChartDto
            {
                Labels = grouped.Select(g => g.Label).ToList(),
                Data = grouped.Select(g => g.Count).ToList()
            };
        }

        public async Task<BookingEntity?> GetByEvoucherCodeAsync(string code)
        {
            return await _bookingRepository.GetAsync(
                b => b.EvoucherId == code,
                b => b.Tickets
            );
        }
    }
}
