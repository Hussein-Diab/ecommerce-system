namespace ECommerce.Contracts.Messages;

using ECommerce.Contracts.Enums;
public class OrderStatusChanged : IntegrationMessage
{
    public required Guid OrderId { get; set; }
    public required string CustomerId { get; set; }
    public OrderStatus NewStatus { get; set; }
    public DateTime? EstimatedDeliveryTime { get; set; }
    public string? Note { get; set; }
    public string? TrackingNumber { get; set; }
}