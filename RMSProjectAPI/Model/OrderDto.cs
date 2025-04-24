using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RMSProjectAPI.Database.Entity;

namespace RMSProjectAPI.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public OrderType Type { get; set; }
        public decimal Price { get; set; }
        public decimal DeliveryFee { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Address { get; set; }
        public string? PaymentSystem { get; set; }
        public string TransactionId { get; set; }
        public string? Note { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? DeliveryId { get; set; }
        public Guid? WaiterId { get; set; }
        public Guid? TableId { get; set; }
        public TimeSpan EstimatedPreparationTime { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }

    public class CreateOrderDto
    {
        public OrderType Type { get; set; }
        public string PaymentSystem { get; set; }
        public string TransactionId { get; set; }
        public decimal? DeliveryFee { get; set; }
        public string? Note { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? DeliveryId { get; set; }
        public Guid? WaiterId { get; set; }
        public Guid? TableId { get; set; }
        public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
    }

    public class UpdateOrderStatusDto
    {
        public string Status { get; set; }
    }

    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public SpicyLevel SpicyLevel { get; set; }
        public decimal Price { get; set; }

        public Guid MenuItemId { get; set; }
        public string MenuItemName { get; set; }

        public Guid MenuItemSizeId { get; set; }
        public decimal MenuItemSizePrice { get; set; }
    }

    public class CreateOrderItemDto
    {
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public SpicyLevel SpicyLevel { get; set; }
        public Guid MenuItemId { get; set; }
        public Guid MenuItemSizeId { get; set; }

        public List<Guid> ExtraIds { get; set; }
    }

    public class OrderTimeDto
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public TimeSpan EstimatedPreparationTime { get; set; }
        public DateTime ExpectedReadyTime => OrderDate + EstimatedPreparationTime;
    }
}