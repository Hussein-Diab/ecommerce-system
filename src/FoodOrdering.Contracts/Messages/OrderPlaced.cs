
namespace FoodOrdering.Contracts.Messages;
public class OrderPlaced : IntegrationMessage
{
    public Guid OrderId { get; set; } = Guid.NewGuid();
    public string CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public List<OrderPlacedItem> Items { get; set; } = [];
    public decimal TotalAmount {get;set;}

}