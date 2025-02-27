using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class Booking
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }

        public string CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public virtual User Customer { get; set; }

        public Guid TableId { get; set; }
        [ForeignKey(nameof(TableId))]
        public virtual Table Table { get; set; }
    }
}
