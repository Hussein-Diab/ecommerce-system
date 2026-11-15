using System.Runtime.CompilerServices;

public class OrderPlaced : IntegrationMessage
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public String CustomerName { get; set; } = string.Empty;
    public String DeliveryAddress { get; set; } = string.Empty;
    public int PhoneNumber { get; set; }
    public List<OrderPlacedItem> items { get; set; } = [];

}