namespace ECommerce.Client.Web.Models.Dtos;

public class OrderDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string ShippingAddress { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime? EstimatedDeliveryTime { get; set; }
    public string? TrackingNumber { get; set; }
    public string? StatusNote { get; set; }
    public bool IsPaid { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();

}