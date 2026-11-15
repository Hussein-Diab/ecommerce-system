namespace ECommerce.Contracts.Messages;
public abstract class IntegrationMessage
{
    public  Guid MessageId { get; set; } = Guid.NewGuid();
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
}