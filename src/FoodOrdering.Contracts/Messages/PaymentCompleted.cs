namespace FoodOrdering.Contracts.Messages;
public class PaymentCompleted : IntegrationMessage
{
    public Guid OrderId { get; set; } = Guid.NewGuid();
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;


}