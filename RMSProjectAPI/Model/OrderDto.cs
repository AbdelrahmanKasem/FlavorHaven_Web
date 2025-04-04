using RMSProjectAPI.Database.Entity;

namespace RMSProjectAPI.Model
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public OrderType Type { get; set; }
        public decimal Price { get; set; }
        public string PaymentSystem { get; set; }
        public string? Note { get; set; }
        public Guid CustomerId { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}