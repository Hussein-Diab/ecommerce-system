public class OrderStatusChanged : IntegrationMessage
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public OrderStatus NewStatus { get; set; }
    public DateTime? EstimatedDeliveryTime { get; set; }
    public String? Note { get; set; }
}