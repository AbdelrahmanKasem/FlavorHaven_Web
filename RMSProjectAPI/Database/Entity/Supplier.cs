using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class Supplier
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }

        public Guid SupervisorId { get; set; }
        [ForeignKey(nameof(SupervisorId))]
        public virtual User Supervisor { get; set; }
    }
}
