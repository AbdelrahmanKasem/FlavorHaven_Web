namespace RMSProjectAPI.Database.Entity
{
    public class Table
    {
        public Guid Id { get; set; }
        public bool IsAvailable { get; set; }
        public int Capacity { get; set; }
    }
}
