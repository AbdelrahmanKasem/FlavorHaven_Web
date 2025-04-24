using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RMSProjectAPI.Database.Entity
{
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public SpicyLevel SpicyLevel { get; set; }
        public decimal Price { get; set; }

        public virtual List<OrderItemExtra> OrderItemExtras { get; set; } = new();

        [Required]
        public Guid MenuItemId { get; set; }

        [Required]
        public Guid MenuItemSizeId { get; set; }

        [ForeignKey("MenuItemId")]
        public MenuItem MenuItem { get; set; }

        [ForeignKey("MenuItemSizeId")]
        public MenuItemSize MenuItemSize { get; set; }

        [Required]
        public Guid OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SpicyLevel { None, Mild, Medium, Hot, ExtraHot }
}