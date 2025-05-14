using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Business.Dto
{
    public class BookingDto
    {
        public string InvoiceId { get; set; }
        public string BookingName { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid EventId { get; set; }

        public List<BookedTicketDto> Tickets { get; set; } = new();
    }

    public class BookedTicketDto
    {
        public Guid TicketTypeId { get; set; }
        public string TicketType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PricePerTicket { get; set; }
    }
}
