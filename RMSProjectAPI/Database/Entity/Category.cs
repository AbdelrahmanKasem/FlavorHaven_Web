using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal? Offers { get; set; }
    }
}
