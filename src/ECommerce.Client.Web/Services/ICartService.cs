using ECommerce.Client.Web.Models.Cart;
using ECommerce.Client.Web.Models.Dtos;

namespace ECommerce.Client.Web.Services;

public interface ICartService
{
    List<CartItem> GetItems();
    int GetCount();                      // total number of units (for the navbar badge)
    decimal GetTotal();                  // display-only total
    int GetQuantity(int productId);      // how many of this product are already in the cart
    void Add(ProductDto product, int quantity);
    void UpdateQuantity(int productId, int quantity);   // 0 or less removes the item
    void Remove(int productId);
    void Clear();
}
