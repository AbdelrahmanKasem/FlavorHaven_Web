using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class FavoriteMeal
    {
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [Required]
        public Guid MenuItemId { get; set; }
        [ForeignKey(nameof(MenuItemId))]
        public virtual MenuItem MenuItem { get; set; }
    }
}
