namespace RMSProjectAPI.Model
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal? Offers { get; set; }
        public Guid MenuId { get; set; }
    }
}
