using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class MenuItem
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string ImagePath { get; set; }
        [Required]
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }

        // Rating system
        public int TotalRating { get; set; } = 0; // Sum of all ratings
        public int RatingCount { get; set; } = 0; // Number of ratings received

        [NotMapped]
        public double AverageRating => RatingCount > 0 ? (double)TotalRating / RatingCount : 0;

        [Required]
        public Guid CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }

        public virtual List<Extra> Extras { get; set; }
        public List<MenuItemSize> Sizes { get; set; }
    }
}
