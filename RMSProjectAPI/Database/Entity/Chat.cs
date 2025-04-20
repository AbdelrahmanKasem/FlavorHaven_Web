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
        [ForeignKey(nameof(User1))]
        public Guid User1ID { get; set; }

        [Required]
        [ForeignKey(nameof(User2))]
        public Guid User2ID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}
