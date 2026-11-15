using System.Diagnostics.Contracts;

public class OrderPlacedItem
{
    public int MenuItemId { get; set; }
    public String Name { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}