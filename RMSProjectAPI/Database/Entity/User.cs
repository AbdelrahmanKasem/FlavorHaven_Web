using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class User: IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly? BirthDate { get; set; }
        public char? Gender { get; set; }
        public DateOnly CreatedAt { get; set; }
        public string? Status { get; set; }
        public string? ImagePath { get; set; }

        public virtual List<Order> Orders { get; set; }
        public virtual List<Address> Addresses { get; set; } = new();
    }
}
