using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RMSProjectAPI.Database.Entity
{
    public class GroupOrder
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsSubmitted { get; set; } = false;

        public Guid AdminUserId { get; set; }
        [ForeignKey(nameof(AdminUserId))]
        public virtual User AdminUser { get; set; }

        public virtual List<GroupOrderMember> Members { get; set; } = new List<GroupOrderMember>();
    }
}
