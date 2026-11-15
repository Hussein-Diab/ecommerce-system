public class MenuItemChanged : IntegrationMessage
{
    public int MenuItemId {get;set;}
    public MenuChangeType ChangeType {get;set;}
    public String Name {get;set;}=string.Empty;
    public String Description {get;set;}=string.Empty;
    public decimal Price {get;set;}
    public String Category {get;set;} = string.Empty;
    public bool IsAvailable ;

}