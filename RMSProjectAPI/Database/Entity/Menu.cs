using System.ComponentModel.DataAnnotations;

namespace RMSProjectAPI.Database.Entity
{
    public class Menu
    {
        [Key]
        public Guid Id { get; set; }
        public decimal? Offers { get; set; }
    }
}
