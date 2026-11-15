namespace ECommerce.Contracts.Messages;

using ECommerce.Contracts.Enums;
public class ProductChanged : IntegrationMessage
{
    public required int ProductId { get; set; }
    public ProductChangeType ChangeType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public int StockQuantity { get; set; }

}