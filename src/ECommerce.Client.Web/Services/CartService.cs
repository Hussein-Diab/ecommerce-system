using System.Text.Json;
using ECommerce.Client.Web.Models.Cart;
using ECommerce.Client.Web.Models.Dtos;

namespace ECommerce.Client.Web.Services;

// The cart lives in the SESSION: server memory, one per browser.
// The browser only holds the session id cookie, never the cart data.
// Business limits (availability, stock) are checked by CartController, not here:
// this class only manages the list.
public class CartService : ICartService
{
    private const string CartKey = "Cart";
    private readonly IHttpContextAccessor _accessor;

    public CartService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    // The current browser's session
    private ISession Session => _accessor.HttpContext!.Session;

    // ---- Load / save the whole cart as JSON (the session only stores text) ----
    private List<CartItem> Load()
    {
        var json = Session.GetString(CartKey);
        return json is null
            ? new List<CartItem>()
            : JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
    }

    private void Save(List<CartItem> cart) =>
        Session.SetString(CartKey, JsonSerializer.Serialize(cart));

    // ---- Reading -------------------------------------------------------------
    public List<CartItem> GetItems() => Load();

    public int GetCount() => Load().Sum(i => i.Quantity);

    public decimal GetTotal() => Load().Sum(i => i.LineTotal);

    public int GetQuantity(int productId) =>
        Load().FirstOrDefault(i => i.ProductId == productId)?.Quantity ?? 0;

    // ---- Changing: always Load → change → Save ----------------------------------
    public void Add(ProductDto product, int quantity)
    {
        var cart = Load();
        var existing = cart.FirstOrDefault(i => i.ProductId == product.Id);

        if (existing is not null)
        {
            existing.Quantity += quantity;          // already in the cart: increase, no duplicate line
            existing.Price = product.Price;         // refresh with the latest data from the API
            existing.Name = product.Name;
            existing.ImageUrl = product.ImageUrl;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Quantity = quantity
            });
        }

        Save(cart);
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var cart = Load();
        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
            return;

        if (quantity <= 0)
            cart.Remove(item);                      // 0 means "remove"
        else
            item.Quantity = quantity;

        Save(cart);
    }

    public void Remove(int productId)
    {
        var cart = Load();
        cart.RemoveAll(i => i.ProductId == productId);
        Save(cart);
    }

    public void Clear() => Session.Remove(CartKey);
}
