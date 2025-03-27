using System.ComponentModel.DataAnnotations;

namespace RMSProjectAPI.Model
{
    public class BranchDto
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Street { get; set; }

        public string GoogleMapsLocation { get; set; }

        [Required]
        public Guid ManagerId { get; set; }
    }
}
