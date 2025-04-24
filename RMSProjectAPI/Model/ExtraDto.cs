namespace RMSProjectAPI.Model
{
    public class ExtraDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public Guid MenuItemId { get; set; }
    }

    public class UpdateExtraDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public Guid MenuItemId { get; set; }
    }
}
