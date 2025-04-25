using Microsoft.EntityFrameworkCore;
using RMSProjectAPI.Database.Entity;
using System.ComponentModel.DataAnnotations;

[Index(nameof(TableNumber), IsUnique = true)]
public class Table
{
    public Guid Id { get; set; }
    [Required]
    public int TableNumber { get; set; }

    public bool IsAvailable { get; set; }
    public int Capacity { get; set; }
    public string QrCodeUrl { get; set; }
    public byte[] QrCodeImage { get; set; }

    public virtual List<Booking> Bookings { get; set; } = new List<Booking>();
}
