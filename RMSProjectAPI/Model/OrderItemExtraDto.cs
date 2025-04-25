namespace RMSProjectAPI.Model
{
    public class OrderItemExtraDto
    {
        public Guid Id { get; set; }
        public Guid ExtraId { get; set; }
        public string ExtraName { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
    }
}
