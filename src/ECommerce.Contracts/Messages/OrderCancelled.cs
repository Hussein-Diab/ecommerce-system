
namespace ECommerce.Contracts.Messages;
public class OrderCancelled : IntegrationMessage
{
    public required Guid OrderId { get; set; } 
    
    public required string CustomerId { get; set; } 
    public string Reason { get; set; } = string.Empty;
}