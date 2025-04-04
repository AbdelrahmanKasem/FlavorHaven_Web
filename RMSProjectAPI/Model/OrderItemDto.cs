using RMSProjectAPI.Database.Entity;

namespace RMSProjectAPI.Model
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public SpicyLevel SpicyLevel { get; set; }
        public decimal Price { get; set; }
        public List<OrderItemCustomizationDto> Customizations { get; set; } = new();
    }
}
