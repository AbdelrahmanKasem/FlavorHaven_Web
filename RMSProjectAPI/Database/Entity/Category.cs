using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class Category
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public Guid MenuId { get; set; }
        [ForeignKey(nameof(MenuId))]
        public virtual Menu Menu { get; set; }
    }
}
