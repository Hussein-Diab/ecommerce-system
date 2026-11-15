
namespace ECommerce.Contracts.Messages;
public class OrderPlaced : IntegrationMessage
{
    public required Guid OrderId { get; set; }
    public required string CustomerId { get; set; }= string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public List<OrderPlacedItem> Items { get; set; } = [];
    public decimal TotalAmount {get;set;}

}