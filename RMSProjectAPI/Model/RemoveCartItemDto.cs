namespace RMSProjectAPI.Model
{
    public class RemoveCartItemDto
    {
        public Guid UserId { get; set; }
        public Guid CartItemId { get; set; }
        public Guid MenuItemId { get; internal set; }
    }
}
