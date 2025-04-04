using System.ComponentModel.DataAnnotations;

namespace RMSProjectAPI.Model
{
    public class SupplierDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        [Required]
        public Guid? SupervisorId { get; set; }
    }
}
