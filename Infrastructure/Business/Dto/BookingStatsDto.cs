using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Business.Dto
{
    public class BookingStatsDto
    {
        public int TotalBookings { get; set; }
        public int TotalTicketsSold { get; set; }
        public decimal TotalEarnings { get; set; }
    }
}
