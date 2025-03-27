namespace RMSProjectAPI.Model
{
    public class AddCartItemDto
    {
        public Guid UserId { get; set; } // Identify which user's cart
        public Guid MenuItemId { get; set; } // Instead of ProductId, use MenuItemId
        public int Quantity { get; set; }
    }
}
