using RMSProjectAPI.Database.Entity;
using System.ComponentModel.DataAnnotations.Schema;

public class GroupOrder
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatorId { get; set; } // The user who created the group order
    [ForeignKey(nameof(CreatorId))]
    public virtual User Creator { get; set; }

    public Guid TableId { get; set; } // The table associated with the group order
    [ForeignKey(nameof(TableId))]
    public virtual Table Table { get; set; }

    public decimal TotalPrice { get; set; }
    public string Status { get; set; } // e.g., "Pending", "Paid", "Completed"

    public virtual List<GroupOrderItem> GroupOrderItems { get; set; } // Items in the group order
}