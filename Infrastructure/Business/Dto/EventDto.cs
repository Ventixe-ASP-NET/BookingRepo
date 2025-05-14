using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Business.Dto
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string EventName { get; set; } = string.Empty;
        public List<TicketTypeDto> TicketTypes { get; set; } = new();
    }

    public class TicketTypeDto
    {
        public Guid Id { get; set; }
        public string TicketType_ { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int TotalTickets { get; set; }
        public int TicketsSold { get; set; }
        public int TicketsLeft { get; set; }
    }
}
