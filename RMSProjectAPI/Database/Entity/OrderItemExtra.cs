using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class OrderItemExtra
    {
        public Guid Id { get; set; }

        public Guid OrderItemId { get; set; }
        [ForeignKey(nameof(OrderItemId))]
        public virtual OrderItem OrderItem { get; set; }

        public Guid ExtraId { get; set; }
        [ForeignKey(nameof(ExtraId))]
        public virtual Extra Extra { get; set; }

        public decimal Price { get; set; }
    }
}
