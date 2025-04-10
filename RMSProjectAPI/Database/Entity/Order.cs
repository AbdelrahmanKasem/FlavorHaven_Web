using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; } // Enum
        public OrderType Type { get; set; } // Enum
        public decimal Price { get; set; }
        public TimeSpan EstimatedPreparationTime { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentSystem { get; set; }
        public string? Note { get; set; }
        [Required]
        public Guid CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public virtual User Customer { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public enum OrderStatus { Pending, Paid, Completed, Cancelled }
    public enum OrderType { DineIn, TakeAway, Delivery }
}
