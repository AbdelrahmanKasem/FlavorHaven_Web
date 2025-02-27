using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMSProjectAPI.Database.Entity
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public decimal DeliveryPrice { get; set; }
        public decimal? Offers { get; set; }
        public string PaymentSystem { get; set; }
        public string Location { get; set; }
        public string? Note { get; set; }

        public string CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public virtual User Customer { get; set; }
    }
}
