using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class Branch
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string GoogleMapsLocation { get; set; }

        // Rating system
        public int TotalRating { get; set; } = 0; // Sum of all ratings
        public int RatingCount { get; set; } = 0; // Number of ratings received

        [NotMapped]
        public double AverageRating => RatingCount > 0 ? (double)TotalRating / RatingCount : 0;

        public Guid ManagerId { get; set; } // Change from string to Guid
        [ForeignKey(nameof(ManagerId))]
        public virtual User Manager { get; set; }

        public virtual List<Menu> Menus { get; set; }
        public virtual List<User> Employees { get; set; } // Link to employees
    }
}
