namespace RMSProjectAPI.Model
{
    public class SuggestedItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string Descritpion { get; set; }
        public int TotalRating { get; set; }
        public int RatingCount { get; set; }
    }

    public class AddSuggestionDto
    {
        public Guid SuggestedItemId { get; set; }
    }
}
