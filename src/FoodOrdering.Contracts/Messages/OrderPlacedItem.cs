namespace FoodOrdering.Contracts.Messages;

public class OrderPlacedItem
{
    public required int MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}