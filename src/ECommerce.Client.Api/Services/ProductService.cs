using ECommerce.Client.Api.Dtos.Common;
using ECommerce.Client.Api.Dtos.Products;
using ECommerce.Client.Api.Mappings;
using ECommerce.Client.Api.Repositories;
using ECommerce.Client.Api.Services.Common;

namespace ECommerce.Client.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParameters query)
    {
        var (items, totalCount) = await _productRepository.GetAvailablePagedAsync(query);

        return new PagedResult<ProductDto>
        {
            Items = items.Select(p => p.ToDto()).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ServiceResult<ProductDto>> GetProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
            return ServiceResult<ProductDto>.Fail(ServiceErrorType.NotFound, "Product not found.");

        return ServiceResult<ProductDto>.Ok(product.ToDto());
    }
    public Task<List<string>> GetCategoriesAsync() => _productRepository.GetCategoriesAsync();
}