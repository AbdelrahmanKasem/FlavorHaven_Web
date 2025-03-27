namespace RMSProjectAPI.Model
{
    public class FavoriteMealDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid MenuItemId { get; set; }
    }
}
