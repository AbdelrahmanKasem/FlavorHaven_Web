using RMSProjectAPI.Database.Entity;
using System.ComponentModel.DataAnnotations.Schema;

public class GroupOrderItem
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public string Note { get; set; }
    public string SpicyLevel { get; set; }

    public Guid MenuItemId { get; set; }
    [ForeignKey(nameof(MenuItemId))]
    public virtual MenuItem MenuItem { get; set; }

    public Guid GroupOrderId { get; set; }
    [ForeignKey(nameof(GroupOrderId))]
    public virtual GroupOrder GroupOrder { get; set; }
}