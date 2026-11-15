
namespace ECommerce.Contracts.Messages;
public class ReviewSubmitted : IntegrationMessage
{
    public required Guid ReviewId { get; set; }
    public required Guid OrderId { get; set; } 
    public required string CustomerId { get; set; }
    public int Rating { get; set; }
    public string Comment {get;set;} =string.Empty;
}