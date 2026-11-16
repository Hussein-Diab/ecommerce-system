
using ECommerce.Contracts.Enums;
namespace ECommerce.Client.Api.Models.Entities;

public class Order
{
    public Guid Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }   
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? EstimatedDeliveryTime { get; set; }
    public string? TrackingNumber { get; set; }
    public string? StatusNote { get; set; }
    public bool IsPaid { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}