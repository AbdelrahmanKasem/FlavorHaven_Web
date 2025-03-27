namespace RMSProjectAPI.Model
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool IsCheckedOut { get; set; }
        public decimal TotalPrice { get; set; }
        public List<MenuItemDto> Items { get; set; } // Use existing DTO
    }
}
