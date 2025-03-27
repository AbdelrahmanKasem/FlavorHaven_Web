using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class OrderItemCustomization
    {
        public Guid Id { get; set; }
        [Required]
        public Guid OrderItemId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // e.g., "Extra Cheese"
        public decimal ExtraPrice { get; set; }
        [ForeignKey(nameof(OrderItemId))]
        public virtual OrderItem OrderItem { get; set; }
    }
}
