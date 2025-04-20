using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class Message
    {
        public Guid MessageID { get; set; }

        [Required]
        [ForeignKey(nameof(Chat))]
        public Guid ChatID { get; set; }

        [Required]
        [ForeignKey(nameof(Sender))]
        public Guid SenderID { get; set; }

        [Required]
        public string MessageText { get; set; }

        public DateTime SentAt { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;

        public virtual User Sender { get; set; }

        public Chat Chat { get; set; }
    }
}
