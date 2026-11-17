using ECommerce.Client.Web.Models.Dtos;

namespace ECommerce.Client.Web.ApiClients;

public interface IProductApiClient
{
    Task<ApiResult<PagedResult<ProductDto>>> GetProductsAsync(string? category, string? search, int page);
    Task<ApiResult<ProductDto>> GetProductAsync(int id); 
    Task<ApiResult<List<string>>> GetCategoriesAsync();  
}