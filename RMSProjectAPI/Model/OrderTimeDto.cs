namespace RMSProjectAPI.Model
{
    public class OrderTimeDto
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public TimeSpan EstimatedPreparationTime { get; set; }
        public DateTime ExpectedReadyTime => OrderDate + EstimatedPreparationTime;
    }
}
