using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Models
{
    public class EventSnapshotEntity
    {
        [Key]
        public Guid EventId { get; set; }      // Samma ID som från EventService

        public string Name { get; set; }   
        public string Category { get; set; }   
        public string Location { get; set; }   
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
