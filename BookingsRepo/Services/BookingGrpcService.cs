using BookingsGrpcServer.Protos;
using Grpc.Core;
using Infrastructure.Business.Managers;
using Infrastructure.Data.Models;

namespace BookingsGrpcServer.Services
{
    public class BookingGrpcService : BookingService.BookingServiceBase
    {
        private readonly BookingManager _manager;

        public BookingGrpcService(BookingManager manager)
        {
            _manager = manager;
        }

        public override async Task<AddBookingReply> AddBooking(AddBookingRequest request, ServerCallContext context)
        {
            try
            {
                var booking = new BookingEntity
                {
                    InvoiceId = request.InvoiceId,
                    BookingName = request.BookingName,
                    CreatedAt = DateTime.Parse(request.CreatedAt),
                    EventId = Guid.Parse(request.EventId)
                };

                await _manager.AddBookingAsync(booking);

                return new AddBookingReply
                {
                    Success = true,
                    Message = "Booking created successfully"
                };
            }
            catch (Exception ex)
            {
                return new AddBookingReply
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public override async Task<GetAllBookingsReply> GetAllBookings(GetAllBookingsRequest request, ServerCallContext context)
        {
            var bookings = await _manager.GetAllBookingsAsync();

            var reply = new GetAllBookingsReply();

            foreach (var b in bookings)
            {
                reply.Bookings.Add(new BookingModel
                {
                    Id = b.Id,
                    InvoiceId = b.InvoiceId,
                    BookingName = b.BookingName,
                    CreatedAt = b.CreatedAt.ToString("o"),
                    EventId = b.EventId.ToString(),

                    // Hämtas via navigation property
                    EventName = b.EventSnapshot?.Name ?? "",
                    EventLocation = b.EventSnapshot?.Location ?? "",
                    EventStartTime = b.EventSnapshot?.StartTime.ToString("o") ?? "",
                    EventEndTime = b.EventSnapshot?.EndTime.ToString("o") ?? ""
                });
            }

            return reply;
        }
    }

}
