using ECommerce.Client.Web.Models.Cart;

namespace ECommerce.Client.Web.ViewModels;

public class CartViewModel
{
    public List<CartItem> Items { get; set; } = new();
    public decimal Total { get; set; }

    public int ItemCount => Items.Sum(i => i.Quantity);
    public bool IsEmpty => Items.Count == 0;
}
