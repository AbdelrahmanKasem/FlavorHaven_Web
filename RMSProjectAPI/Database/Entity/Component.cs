namespace RMSProjectAPI.Database.Entity
{
    public class Component
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateOnly ExpirationDate { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
    }
}
