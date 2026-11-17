using ECommerce.Client.Web.ApiClients;
using ECommerce.Client.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.Client.Web.Controllers;

public class ProductsController : Controller   // no [Authorize]: anyone can browse
{
    private readonly IProductApiClient _productApi;

    public ProductsController(IProductApiClient productApi)
    {
        _productApi = productApi;
    }

    // GET /Products?category=Electronics&search=mouse&page=2
    [HttpGet]
    public async Task<IActionResult> Index(string? category, string? search, int page = 1)
    {
        if (page < 1)
            page = 1;   // a hand-typed ?page=0 or ?page=-5 just shows page 1

        var model = new ProductListViewModel
        {
            Category = category,
            Search = search
        };

        // 1. The products for this page
        var productsResult = await _productApi.GetProductsAsync(category, search, page);

        if (productsResult.Success)
            model.Products = productsResult.Data!;
        else
            model.ErrorMessage = productsResult.ErrorMessage;   // e.g. "service unavailable"

        // 2. The dropdown options (the current category is pre-selected).
        //    If this call fails, the dropdown is simply empty: it never blocks the products.
        var categoriesResult = await _productApi.GetCategoriesAsync();

        if (categoriesResult.Success)
        {
            model.Categories = categoriesResult.Data!
                .Select(c => new SelectListItem(text: c, value: c, selected: c == category))
                .ToList();
        }

        return View(model);
    }

    // GET /Products/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var result = await _productApi.GetProductAsync(id);

        if (result.StatusCode == 404)
            return NotFound();   // the product doesn't exist

        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage;   // e.g. API down
            return RedirectToAction(nameof(Index));
        }

        return View(new ProductDetailsViewModel { Product = result.Data! });
    }
}
