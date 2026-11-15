public abstract class IntegrationMessage
{
    public Guid MessageId { get; set; }
    public DateTime OccuredAtUtc { get; set; } = DateTime.UtcNow;
}