using RMSProjectAPI.Database.Entity;

namespace RMSProjectAPI.Model
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public TimeSpan Duration { get; set; }
        public BookingStatus Status { get; set; }
        public int GuestCount { get; set; }
        public string TransactionId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid TableId { get; set; }
    }

    public class CreateBookingDto
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public TimeSpan Duration { get; set; }
        public int GuestCount { get; set; }
        public string TransactionId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid TableId { get; set; }
    }

    public class CancelBookingDto
    {
        public Guid BookingId { get; set; }
    }
}
