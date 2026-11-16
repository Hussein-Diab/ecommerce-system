namespace ECommerce.Client.Web.Models.Dtos;

public class PlaceOrderRequest
{

    public string ShippingAddress { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
    public List<PlaceOrderItemRequest> Items { get; set; } = new();
}