namespace FoodOrdering.Contracts.Messages;
public class MenuItemChanged : IntegrationMessage
{
    public int MenuItemId {get;set;}
    public MenuChangeType ChangeType {get;set;}
    public string Name {get;set;}=string.Empty;
    public string Description {get;set;}=string.Empty;
    public decimal Price {get;set;}
    public string Category {get;set;} = string.Empty;
    public bool IsAvailable {get;set;}

}