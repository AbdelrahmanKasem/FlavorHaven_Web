using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public enum BookingStatus { Pending, Confirmed, Cancelled }

    public class Booking
    {
        //public Guid Id { get; set; }
        //public DateTime Date { get; set; }
        //public BookingStatus Status { get; set; } // Use enum for status
        //public DateTime BookingTime { get; set; }

        //public Guid CustomerId { get; set; }
        //[ForeignKey(nameof(CustomerId))]
        //public virtual User? Customer { get; set; } // Mark as nullable

        //public Guid TableId { get; set; }
        //[ForeignKey(nameof(TableId))]
        //public virtual Table? Table { get; set; } // Mark as nullable

        public Guid Id { get; set; }
        public DateTime Date { get; set; } // Stores only the date
        public TimeSpan Time { get; set; } // Stores only the time
        public BookingStatus Status { get; set; }
        public int GuestCount { get; set; } // Number of guests

        public Guid CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public virtual User? Customer { get; set; }

        public Guid TableId { get; set; }
        [ForeignKey(nameof(TableId))]
        public virtual Table? Table { get; set; }
    }
}