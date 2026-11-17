using ECommerce.Client.Api.Dtos.Products;
using ECommerce.Client.Api.Models.Entities;

namespace ECommerce.Client.Api.Mappings;

public static class ProductMappings
{
    // "this Product product" makes it an EXTENSION METHOD: you can write product.ToDto()
    public static ProductDto ToDto(this Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        Category = product.Category,
        StockQuantity = product.StockQuantity,
        IsAvailable = product.IsAvailable,
        ImageUrl = product.ImageUrl
    };
}