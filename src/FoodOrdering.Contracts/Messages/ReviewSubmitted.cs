using System.ComponentModel.DataAnnotations;
public class ReviewSubmitted : IntegrationMessage
{
    public int ReviewId { get; set; }
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    [Range(1, 5)]
    public int Rating { get; set; }
    public String Comment {get;set;} =string.Empty;
}