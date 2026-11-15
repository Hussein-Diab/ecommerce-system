using Mysqlx.Crud;

namespace ECommerce.Client.Api.Models.Entities;

public class OrderItem
{
    public int Id { get; set; }

    public Guid OrderId { get; set; }                // FK (Guid, to match Order.Id)
    public Order Order { get; set; } = null!;        // navigation

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public decimal LineTotal => UnitPrice * Quantity;
}