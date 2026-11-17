using ECommerce.Client.Api.Dtos.Common;
using ECommerce.Client.Api.Dtos.Products;
using ECommerce.Client.Api.Services.Common;

namespace ECommerce.Client.Api.Services;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParameters query);
    Task<ServiceResult<ProductDto>> GetProductAsync(int id);
    Task<List<string>> GetCategoriesAsync();
}