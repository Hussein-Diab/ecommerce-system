namespace FoodOrdering.Contracts.Messages;
public class PaymentCompleted : IntegrationMessage
{
    public required Guid OrderId { get; set; } 
    public required string  TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;


}