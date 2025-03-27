using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class OrderLog
    {
        public Guid Id { get; set; }
        [Required]
        public Guid OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime UpdatedAt { get; set; }
        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }
    }
}
