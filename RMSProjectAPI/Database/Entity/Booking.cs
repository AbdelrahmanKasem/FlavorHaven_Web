using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class Booking
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public TimeSpan Duration { get; set; }
        public BookingStatus Status { get; set; }
        public int GuestCount { get; set; }
        public string TransactionId { get; set; }

        public Guid CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public virtual User? Customer { get; set; }

        public Guid TableId { get; set; }
        [ForeignKey(nameof(TableId))]
        public virtual Table? Table { get; set; }
    }

    public enum BookingStatus { Pending, Confirmed, Cancelled }
}