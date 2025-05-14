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

                //await _manager.AddBookingWithTicketsAsync(booking);

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
                });
            }

            return reply;
        }

        public override async Task<UpdateBookingReply> UpdateBooking(UpdateBookingRequest request, ServerCallContext context)
        {
            try
            {
                var updatedBooking = new BookingEntity
                {
                    Id = request.Id,
                    InvoiceId = request.InvoiceId,
                    BookingName = request.BookingName,
                    CreatedAt = DateTime.Parse(request.CreatedAt),
                    EventId = Guid.Parse(request.EventId)
                };

                var success = await _manager.UpdateBookingAsync(updatedBooking);

                return new UpdateBookingReply
                {
                   // Success = success,
                    //Message = success ? "Booking updated successfully" : "Failed to update booking"
                };
            }
            catch (Exception ex)
            {
                return new UpdateBookingReply
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        public override async Task<DeleteBookingReply> DeleteBooking(DeleteBookingRequest request, ServerCallContext context)
        {
            try
            {
                var success = await _manager.DeleteBookingAsync(request.Id);

                return new DeleteBookingReply
                {
                    //Success = success,
                    //Message = success ? "Booking deleted successfully" : "Failed to delete booking"
                };
            }
            catch (Exception ex)
            {
                return new DeleteBookingReply
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }
    }

}
