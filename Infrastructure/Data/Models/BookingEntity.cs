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
        public int InvoiceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string BookingName { get; set; }

        //FK Till Eventsnapshot
        public Guid EventId { get; set; }

        [ForeignKey(nameof(EventId))]
        public EventSnapshotEntity EventSnapshot { get; set; }
    }
}
