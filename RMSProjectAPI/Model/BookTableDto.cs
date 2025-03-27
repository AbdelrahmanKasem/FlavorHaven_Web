namespace RMSProjectAPI.Model
{
    public class BookTableDto
    {
        public Guid TableId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime BookingTime { get; set; }
    }
}
