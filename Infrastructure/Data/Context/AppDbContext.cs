using Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<BookingEntity> Bookings { get; set; }


    }
}
