using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public SpicyLevel SpicyLevel { get; set; }
        public decimal Price { get; set; }

        [Required]
        public Guid MenuItemId { get; set; }

        [Required]
        public Guid OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }
        public virtual List<OrderItemCustomization> Customizations { get; set; } = new();
    }

    public enum SpicyLevel { None, Mild, Medium, Hot, ExtraHot }
}