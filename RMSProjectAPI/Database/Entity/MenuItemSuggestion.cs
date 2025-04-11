namespace RMSProjectAPI.Database.Entity
{
    public class MenuItemSuggestion
    {
        public Guid Id { get; set; }

        public Guid MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }

        public Guid SuggestedItemId { get; set; }
        public MenuItem SuggestedItem { get; set; }
    }
}
