using System.ComponentModel.DataAnnotations;

namespace RMSProjectAPI.Model
{
    public class UserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string UserName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Street { get; set; }

        public char? Gender { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
       
        public string? Status { get; set; }
        public string? ImagePath { get; set; }
        public string? Region { get; set; }

        public DateOnly CreatedAt { get; set; }
        public string Password { get; set; }
    }
}
