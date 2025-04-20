namespace RMSProjectAPI.Model
{
    public class DeliveryDto
    {
        public string FullName { get; set; }
        public string? ImagePath { get; set; }
        public string PhoneNumber { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
    }
}
