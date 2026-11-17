using ECommerce.Client.Web.Models.Dtos;
using Microsoft.AspNetCore.WebUtilities;   // QueryHelpers

namespace ECommerce.Client.Web.ApiClients;

public class ProductApiClient : IProductApiClient
{
    private readonly HttpClient _http;

    public ProductApiClient(HttpClient http)
    {
        _http = http;
    }

    public Task<ApiResult<PagedResult<ProductDto>>> GetProductsAsync(string? category, string? search, int page)
    {
        var query = new Dictionary<string, string?>
        {
            ["page"] = page.ToString()
        };

        if (!string.IsNullOrWhiteSpace(category))
            query["category"] = category;

        if (!string.IsNullOrWhiteSpace(search))
            query["search"] = search;


        var url = QueryHelpers.AddQueryString("api/products", query);

        return ApiResponseReader.SendAsync<PagedResult<ProductDto>>(() => _http.GetAsync(url));
    }

    public Task<ApiResult<ProductDto>> GetProductAsync(int id)
    {
        return ApiResponseReader.SendAsync<ProductDto>(() => _http.GetAsync($"api/products/{id}"));
    }
    public Task<ApiResult<List<string>>> GetCategoriesAsync()
    {
        return ApiResponseReader.SendAsync<List<string>>(() => _http.GetAsync("api/products/categories"));
    }
}