using Infrastructure.Data.Context;
using Infrastructure.Data.Models;
using Infrastructure.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repository
{
    public class BookingRepository : BaseRepository<BookingEntity>
    {
        public BookingRepository(AppDbContext context) : base(context)
        {
        }
    }

}
