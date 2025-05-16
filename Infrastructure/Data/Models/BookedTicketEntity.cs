using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Data.Models
{
    public class BookedTicketEntity
    {
        public int Id { get; set; }
        public int BookingId { get; set; } // Foreign key till Booking
        [JsonIgnore]
        public BookingEntity Booking { get; set; } = null!; // Navigation

        public Guid TicketTypeId { get; set; }
        public string TicketType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PricePerTicket { get; set; }
    }
}
