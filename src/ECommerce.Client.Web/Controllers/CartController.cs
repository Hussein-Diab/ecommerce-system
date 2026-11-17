using ECommerce.Client.Web.ApiClients;
using ECommerce.Client.Web.Services;
using ECommerce.Client.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Client.Web.Controllers;

public class CartController : Controller   // no [Authorize]: guests can shop; login is required at checkout
{
    private const int MaxPerProduct = 100;  // same limit as the API's [Range(1, 100)]

    private readonly ICartService _cart;
    private readonly IProductApiClient _productApi;

    public CartController(ICartService cart, IProductApiClient productApi)
    {
        _cart = cart;
        _productApi = productApi;
    }

    // GET /Cart
    [HttpGet]
    public IActionResult Index()
    {
        var model = new CartViewModel
        {
            Items = _cart.GetItems(),
            Total = _cart.GetTotal()
        };
        return View(model);
    }

    // POST /Cart/Add   (from the product details page)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        if (quantity < 1 || quantity > MaxPerProduct)
        {
            TempData["Error"] = $"Choose a quantity between 1 and {MaxPerProduct}.";
            return RedirectToAction("Details", "Products", new { id = productId });
        }

        // Load the product FROM THE API: never trust a name or price sent by the form
        var result = await _productApi.GetProductAsync(productId);

        if (!result.Success)
        {
            TempData["Error"] = result.StatusCode == 404
                ? "That product no longer exists."
                : result.ErrorMessage;
            return RedirectToAction("Index", "Products");
        }

        var product = result.Data!;

        if (!product.IsAvailable || product.StockQuantity == 0)
        {
            TempData["Error"] = $"{product.Name} is not available right now.";
            return RedirectToAction("Details", "Products", new { id = productId });
        }

        // Count what's ALREADY in the cart too (3 in cart + 3 more = 6 > stock 5)
        var limit = Math.Min(product.StockQuantity, MaxPerProduct);
        var newQuantity = _cart.GetQuantity(productId) + quantity;

        if (newQuantity > limit)
        {
            TempData["Error"] = $"You can have at most {limit} of {product.Name} in your cart.";
            return RedirectToAction("Details", "Products", new { id = productId });
        }

        _cart.Add(product, quantity);
        TempData["Success"] = $"Added {quantity} × {product.Name} to your cart.";

        return RedirectToAction("Details", "Products", new { id = productId });   // keep shopping
    }

    // POST /Cart/UpdateQuantity   (from the cart page)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        if (quantity > MaxPerProduct)
        {
            TempData["Error"] = $"The maximum is {MaxPerProduct} per product.";
            return RedirectToAction(nameof(Index));
        }

        _cart.UpdateQuantity(productId, quantity);   // 0 or less removes the item
        TempData["Success"] = quantity <= 0 ? "Item removed." : "Cart updated.";

        return RedirectToAction(nameof(Index));
    }

    // POST /Cart/Remove
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int productId)
    {
        _cart.Remove(productId);
        TempData["Success"] = "Item removed.";
        return RedirectToAction(nameof(Index));
    }

    // POST /Cart/Clear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        _cart.Clear();
        TempData["Success"] = "Your cart is empty.";
        return RedirectToAction(nameof(Index));
    }
}
