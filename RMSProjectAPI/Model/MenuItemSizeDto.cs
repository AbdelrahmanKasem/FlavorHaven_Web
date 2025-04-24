namespace RMSProjectAPI.Model
{
    public class MenuItemSizeDto
    {
        public Guid Id { get; set; }
        public int Grams { get; set; }
        public decimal Price { get; set; }
        public Guid MenuItemId { get; set; }
    }

    public class UpdateMenuItemSizeDto
    {
        public int Grams { get; set; }
        public decimal Price { get; set; }
        public Guid MenuItemId { get; set; }
    }
}
