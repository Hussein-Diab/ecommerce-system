namespace FoodOrdering.Contracts.Messages;
public class OrderStatusChanged : IntegrationMessage
{
    public Guid OrderId { get; set; } = Guid.NewGuid();
    public string CustomerId { get; set; }
    public OrderStatus NewStatus { get; set; }
    public DateTime? EstimatedDeliveryTime { get; set; }
    public string? Note { get; set; }
}