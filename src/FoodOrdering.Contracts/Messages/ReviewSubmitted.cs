
namespace FoodOrdering.Contracts.Messages;
public class ReviewSubmitted : IntegrationMessage
{
    public Guid ReviewId { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; } = Guid.NewGuid();
    public int CustomerId { get; set; }
    public int Rating { get; set; }
    public string Comment {get;set;} =string.Empty;
}