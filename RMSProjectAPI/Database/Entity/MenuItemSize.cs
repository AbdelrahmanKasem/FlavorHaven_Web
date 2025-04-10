namespace RMSProjectAPI.Database.Entity
{
    public class MenuItemSize
    {
        public Guid Id { get; set; }
        public Guid MenuItemId { get; set; }
        public int Grams { get; set; }
        public decimal Price { get; set; }

        public MenuItem MenuItem { get; set; }
    }
}
