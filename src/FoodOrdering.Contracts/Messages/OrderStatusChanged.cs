namespace FoodOrdering.Contracts.Messages;
using FoodOrdering.Contracts.Enums;
public class OrderStatusChanged : IntegrationMessage
{
    public required Guid OrderId { get; set; } 
    public required string CustomerId { get; set; } = string.Empty;
    public OrderStatus NewStatus { get; set; }
    public DateTime? EstimatedDeliveryTime { get; set; }
    public string? Note { get; set; }
}