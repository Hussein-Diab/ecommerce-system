using System.Diagnostics.Contracts;

public class OrderCancelled : IntegrationMessage
{
    public int OrderId {get;set;}
    public int CustomerId {get;set;}
    public String Reason {get;set;}= string.Empty;
}