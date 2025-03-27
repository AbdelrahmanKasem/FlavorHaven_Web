using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class CartItem
    {
        public Guid Id { get; set; }

        public Guid CartId { get; set; }
        [ForeignKey(nameof(CartId))]
        public virtual Cart Cart { get; set; }

        public Guid MenuItemId { get; set; }
        [ForeignKey(nameof(MenuItemId))]
        public virtual MenuItem MenuItem { get; set; }

        public int Quantity { get; set; }

        public decimal PriceAtTimeOfOrder { get; set; } // Store historical price

        [NotMapped]
        public decimal TotalPrice => PriceAtTimeOfOrder * Quantity;
    }
}
