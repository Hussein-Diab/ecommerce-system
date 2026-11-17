using ECommerce.Client.Api.Dtos.Products;
using ECommerce.Client.Api.Models.Entities;

namespace ECommerce.Client.Api.Repositories;

public interface IProductRepository
{
    Task<(List<Product> Items, int TotalCount)> GetAvailablePagedAsync(ProductQueryParameters query);

    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids);
    Task<List<string>> GetCategoriesAsync();
}