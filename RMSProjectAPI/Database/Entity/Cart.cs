using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class Cart
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public bool IsCheckedOut { get; set; } = false;

        public virtual List<CartItem> Items { get; set; } = new List<CartItem>();

        [NotMapped]
        public decimal TotalPrice { get; set; }
    }
}
