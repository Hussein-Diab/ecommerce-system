using System.ComponentModel.DataAnnotations;
namespace ECommerce.Client.Api.Dtos.Orders;

public class PlaceOrderRequest
{
    [Required]
    [MaxLength(300)]
    public string ShippingAddress { get; set; } = string.Empty;
    [Phone]
    [MaxLength(20)]
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required]
    [MinLength(1)]
    public List<PlaceOrderItemRequest> Items { get; set; } = new();
}