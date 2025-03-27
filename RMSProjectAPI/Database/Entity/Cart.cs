using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class Cart
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; } // User who owns the cart

        public bool IsCheckedOut { get; set; } = false; // True if order is placed

        public virtual List<CartItem> Items { get; set; } = new List<CartItem>();

        [NotMapped]
        public decimal TotalPrice => Items.Sum(i => i.TotalPrice);
    }
}
