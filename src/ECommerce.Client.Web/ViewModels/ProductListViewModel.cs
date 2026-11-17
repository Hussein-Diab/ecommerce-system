using ECommerce.Client.Web.Models.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;   // SelectListItem

namespace ECommerce.Client.Web.ViewModels;

// Everything the product list page needs, in one object
public class ProductListViewModel
{
    // The current page of products (from the API)
    public PagedResult<ProductDto> Products { get; set; } = new();

    // The current filters, so the form stays filled and page links keep them
    public string? Category { get; set; }
    public string? Search { get; set; }

    // Options for the category dropdown
    public List<SelectListItem> Categories { get; set; } = new();

    // Set when the API couldn't be reached
    public string? ErrorMessage { get; set; }

    public bool HasFilters => !string.IsNullOrWhiteSpace(Category) || !string.IsNullOrWhiteSpace(Search);
}
