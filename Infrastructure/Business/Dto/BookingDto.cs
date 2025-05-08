using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Business.Dto
{
    public class BookingDto
    {
        public int InvoiceId { get; set; }
        public string BookingName { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid EventId { get; set; }
    }
}
