using System.ComponentModel.DataAnnotations;

namespace RMSProjectAPI.Model
{
    public class MenuDto
    {
        public Guid Id { get; set; }
        public decimal? Offers { get; set; }
    }
}
