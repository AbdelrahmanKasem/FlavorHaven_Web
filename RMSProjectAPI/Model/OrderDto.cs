// DTOs/OrderDto.cs
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
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Address { get; set; }
        public string? PaymentSystem { get; set; }
        public string TransactionId { get; set; }
        public string? Note { get; set; }
        public Guid CustomerId { get; set; }
        public TimeSpan EstimatedPreparationTime { get; set; } // Add this
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }

    public class CreateOrderDto
    {
        public OrderType Type { get; set; }
        public string PaymentSystem { get; set; }
        public string TransactionId { get; set; }
        public string? Note { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
        public Guid CustomerId { get; set; }
        public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
    }

    public class UpdateOrderStatusDto
    {
        public string Status { get; set; }
    }
}

// DTOs/OrderItemDto.cs
namespace RMSProjectAPI.DTOs
{
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
        public string MenuItemSizeName { get; set; }  // Size name (e.g., "Small", "Medium")
        public decimal MenuItemSizePrice { get; set; }
    }

    public class CreateOrderItemDto
    {
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public SpicyLevel SpicyLevel { get; set; }
        public Guid MenuItemId { get; set; }
        public Guid MenuItemSizeId { get; set; } // Add MenuItemSizeId
    }
}