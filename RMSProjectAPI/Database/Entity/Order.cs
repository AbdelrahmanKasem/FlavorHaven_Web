using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RMSProjectAPI.Database.Entity
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; } 
        public OrderType Type { get; set; }
        public decimal Price { get; set; }
        public decimal DeliveryFee { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Address { get; set; }
        public TimeSpan EstimatedPreparationTime { get; set; }
        public string TransactionId { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentSystem { get; set; } = "Online";
        public string? Note { get; set; }
        [Required]
        public Guid CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public virtual User Customer { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<OrderLog> OrderLogs { get; set; }

        // Waiter
        public Guid? WaiterId { get; set; }
        [ForeignKey(nameof(WaiterId))]
        public virtual User? Waiter { get; set; }

        // Delivery Person
        public Guid? DeliveryId { get; set; }
        [ForeignKey(nameof(DeliveryId))]
        public virtual User? DeliveryPerson { get; set; }

        // Cashier Person
        public Guid? CashierId { get; set; }
        [ForeignKey(nameof(CashierId))]
        public virtual User? CashierPerson { get; set; }

        public Guid? TableId { get; set; }

        [ForeignKey(nameof(TableId))]
        public virtual Table? Table { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus { Pending, Paid, InProgress, Ready, Completed, Cancelled }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderType { DineIn, TakeAway, Delivery }
}