using Infrastructure.Business.Dto;
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

        public BookingManager(BookingRepository bookingRepository, EventServiceClient eventClient)
        {
            _bookingRepository = bookingRepository;
            _eventClient = eventClient;
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

                var booking = new BookingEntity
                {
                    InvoiceId = dto.InvoiceId,
                    BookingName = dto.BookingName,
                    CreatedAt = dto.CreatedAt,
                    EventId = dto.EventId,
                    Tickets = dto.Tickets.Select(t => new BookedTicketEntity
                    {
                        TicketTypeId = t.TicketTypeId,
                        TicketType = t.TicketType,
                        Quantity = t.Quantity,
                        PricePerTicket = t.PricePerTicket
                    }).ToList()
                };

                var result = await _bookingRepository.AddAsync(booking);
                return result ? (true, null) : (false, "Could not save booking to database");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in AddBookingWithTicketsAsync: {ex.Message}");
                return (false, "Unexpected error while adding booking");
            }
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
    }
}
