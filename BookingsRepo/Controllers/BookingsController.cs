using Infrastructure.Business.Dto;
using Infrastructure.Business.Managers;
using Infrastructure.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookingsGrpcServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : Controller
    {
        private readonly BookingManager _manager;

        public BookingsController(BookingManager manager)
        {
            _manager = manager;
        }

        // GET: api/bookings
        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _manager.GetAllBookingsAsync();
            return Ok(bookings);
        }

        // POST: api/bookings
        [HttpPost]
        public async Task<IActionResult> AddBooking([FromBody] BookingDto dto)
        {
            var booking = new BookingEntity
            {
                InvoiceId = dto.InvoiceId,
                BookingName = dto.BookingName,
                CreatedAt = dto.CreatedAt,
                EventId = dto.EventId
            };

            var success = await _manager.AddBookingAsync(booking);

            if (!success)
                return BadRequest("Could not add booking");

            return Ok("Booking created");
        }

        // PUT: api/bookings/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingDto dto)
        {
            var updatedBooking = new BookingEntity
            {
                Id = id, // sätts från route
                InvoiceId = dto.InvoiceId,
                BookingName = dto.BookingName,
                CreatedAt = dto.CreatedAt,
                EventId = dto.EventId
            };

            var success = await _manager.UpdateBookingAsync(updatedBooking);

            if (!success)
                return NotFound("Could not update booking (not found or failed)");

            return Ok("Booking updated");
        }

        // DELETE: api/bookings/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var success = await _manager.DeleteBookingAsync(id);
            if (!success)
                return NotFound("Booking not found or could not be deleted");

            return Ok("Booking deleted");
        }
    }
}
