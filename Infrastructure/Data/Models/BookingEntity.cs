using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Models
{
    public class BookingEntity
    {
        [Key]
        public int Id { get; set; }
        public string InvoiceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string BookingName { get; set; }
        public Guid EventId { get; set; }
        public ICollection<BookedTicketEntity> Tickets { get; set; } = new List<BookedTicketEntity>();

        public string EvoucherId { get; set; }
    }
}
