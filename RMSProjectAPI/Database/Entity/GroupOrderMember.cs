using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RMSProjectAPI.Database.Entity
{
    public class GroupOrderMember
    {
        [Key]
        public Guid Id { get; set; }
        public Guid GroupOrderId { get; set; }
        [ForeignKey(nameof(GroupOrderId))]
        public virtual GroupOrder GroupOrder { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public virtual List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
