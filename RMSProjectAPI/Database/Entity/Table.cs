using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class Table
    {
        public Guid Id { get; set; }
        public bool IsAvailable { get; set; }
        public int Capacity { get; set; }
        public string QrCodeUrl { get; set; }
        public byte[] QrCodeImage { get; set; }

        //[Required]
        //public Guid BranchId { get; set; }
        //[ForeignKey(nameof(BranchId))]
        //public virtual Branch Branch { get; set; }

        public virtual List<Booking> Bookings { get; set; } = new List<Booking>();
    }
}