using Infrastructure.Business.Dto;
using Infrastructure.Business.Managers;
using Infrastructure.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookingsGrpcServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly BookingManager _manager;

        public BookingsController(BookingManager manager)
        {
            _manager = manager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            try
            {
                var bookings = await _manager.GetAllBookingsAsync();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAllBookings error: {ex.Message}");
                return StatusCode(500, "Unexpected error while retrieving bookings");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddBooking([FromBody] BookingDto dto)
        {
            var (success, error) = await _manager.AddBookingWithTicketsAsync(dto);

            if (!success)
                return BadRequest(error);

            return Ok("Booking created");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingDto dto)
        {
            var updatedBooking = new BookingEntity
            {
                Id = id,
                InvoiceId = dto.InvoiceId,
                BookingName = dto.BookingName,
                CreatedAt = dto.CreatedAt,
                EventId = dto.EventId
            };

            var (success, message) = await _manager.UpdateBookingAsync(updatedBooking);

            if (!success)
                return NotFound(message);

            return Ok("Booking updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var (success, message) = await _manager.DeleteBookingAsync(id);

            if (!success)
                return NotFound(message);

            return Ok("Booking deleted");
        }
    }
}
