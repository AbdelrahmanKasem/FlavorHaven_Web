namespace RMSProjectAPI.Model
{
    public class BookTableDto
    {
        //public Guid TableId { get; set; }
        //public Guid CustomerId { get; set; }
        //public DateTime BookingTime { get; set; }

        public Guid TableId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public int GuestCount { get; set; }
        public Guid BranchId { get; set; }
    }
}
