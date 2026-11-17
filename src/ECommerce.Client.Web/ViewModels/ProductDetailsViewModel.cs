using System.ComponentModel.DataAnnotations;
using ECommerce.Client.Web.Models.Dtos;

namespace ECommerce.Client.Web.ViewModels;

public class ProductDetailsViewModel
{
    public ProductDto Product { get; set; } = new();

    // The "Add to cart" form field
    [Range(1, 100, ErrorMessage = "Choose a quantity between 1 and 100.")]
    public int Quantity { get; set; } = 1;

    // One place for the "can this be bought?" rule, so the view doesn't repeat it
    public bool CanBuy => Product.IsAvailable && Product.StockQuantity > 0;
}
