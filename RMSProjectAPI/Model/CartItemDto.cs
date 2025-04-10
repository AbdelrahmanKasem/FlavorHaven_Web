namespace RMSProjectAPI.Model
{
    public class CartItemDto
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public string MenuItemImage { get; set; }
        public string MenuItemDescription { get; set; }
        public Guid MenuItemSizeId { get; set; } // The selected size
        public decimal PriceAtTimeOfOrder { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => PriceAtTimeOfOrder * Quantity;
    }
}
