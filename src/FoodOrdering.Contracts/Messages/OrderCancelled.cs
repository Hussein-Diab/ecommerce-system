
namespace FoodOrdering.Contracts.Messages;
public class OrderCancelled : IntegrationMessage
{
    public Guid OrderId { get; set; } = Guid.NewGuid();
    public string CustomerId { get; set; }
    public string Reason { get; set; } = string.Empty;
}