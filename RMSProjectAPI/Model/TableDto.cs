namespace RMSProjectAPI.Model
{
    public class TableDto
    {
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
    }

    public class TableStatusDto
    {
        public Guid TableId { get; set; }
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; }
    }

}