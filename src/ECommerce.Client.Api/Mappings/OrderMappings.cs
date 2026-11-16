using ECommerce.Client.Api.Dtos.Orders;
using ECommerce.Client.Api.Models.Entities;

namespace ECommerce.Client.Api.Mappings;

public static class OrderMappings
{
    public static OrderItemDto ToDto(this OrderItem i) => new()
    {
        ProductId = i.ProductId,
        ProductName = i.ProductName,
        UnitPrice = i.UnitPrice,
        Quantity = i.Quantity,
        LineTotal = i.LineTotal
    };


    public static OrderDto ToDto(this Order order) => new()
    {
        Id = order.Id,
        Status = order.Status.ToString(),
        TotalAmount = order.TotalAmount,
        CreatedAtUtc = order.CreatedAtUtc,
        ShippingAddress = order.ShippingAddress,
        PhoneNumber = order.PhoneNumber,
        EstimatedDeliveryTime = order.EstimatedDeliveryTime,
        TrackingNumber = order.TrackingNumber,
        StatusNote = order.StatusNote,
        IsPaid = order.IsPaid,
        Items = order.Items.Select(i => i.ToDto()).ToList()
    };
}