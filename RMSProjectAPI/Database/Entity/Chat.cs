using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Intrinsics.X86;

namespace RMSProjectAPI.Database.Entity
{
    public class Chat
    {
        [Key]
        public Guid ChatID { get; set; }

        [Required]
        public Guid User1ID { get; set; }

        [Required]
        public Guid User2ID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Message> Messages { get; set; }
    }
}
