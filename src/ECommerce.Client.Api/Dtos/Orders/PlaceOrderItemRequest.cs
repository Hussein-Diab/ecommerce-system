using System.ComponentModel.DataAnnotations;
namespace ECommerce.Client.Api.Dtos.Orders;

public class PlaceOrderItemRequest
{
    public int ProductId { get; set; }
    [Range(1, 100)]
    public int Quantity { get; set; }
}