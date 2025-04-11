namespace RMSProjectAPI.Model
{
    public class SuggestedItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }

    public class AddSuggestionDTO
    {
        public Guid SuggestedItemId { get; set; }
    }
}
