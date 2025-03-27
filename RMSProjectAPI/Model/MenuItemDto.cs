using RMSProjectAPI.Database.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Model
{
    public class MenuItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public decimal? Offers { get; set; }
        public Guid CategoryId { get; set; }
        public int Quantity { get; internal set; }
        public decimal TotalPrice { get; internal set; }
    }
}
