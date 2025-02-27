using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class Branch
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }

        public string ManagerId { get; set; }
        [ForeignKey(nameof(ManagerId))]
        public virtual User Manager { get; set; }
    }
}
