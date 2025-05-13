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

        public BookingManager(BookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<bool> AddBookingAsync(BookingEntity booking)
        {
            return await _bookingRepository.AddAsync(booking);
        }

        public async Task<List<BookingEntity>> GetAllBookingsAsync()
        {
            return (await _bookingRepository.GetAllAsync()).ToList();
        }

        public async Task<bool> UpdateBookingAsync(BookingEntity updatedBooking)
        {
            return await _bookingRepository.UpdateAsync(updatedBooking);
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            return await _bookingRepository.DeleteAsync(b => b.Id == id);
        }
    }
}
