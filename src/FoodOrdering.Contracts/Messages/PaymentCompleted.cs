public class PaymentCompleted : IntegrationMessage
{
    public int OrderId {get;set;}
    public int TransactionId {get;set;}
    public decimal Amount {get;set;}
    public String PaymentMethod {get;set;}= string.Empty;
    

}