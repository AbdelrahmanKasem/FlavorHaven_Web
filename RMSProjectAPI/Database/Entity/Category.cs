using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal? Offers { get; set; }

        public Guid MenuId { get; set; }
        [ForeignKey(nameof(MenuId))]
        public virtual Menu Menu { get; set; }

        public virtual List<MenuItem> MenuItems { get; set; } // Link to menu items
    }
}
