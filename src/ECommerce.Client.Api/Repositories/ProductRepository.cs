using ECommerce.Client.Api.Data;
using ECommerce.Client.Api.Dtos.Products;
using ECommerce.Client.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Client.Api.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _db;

    public ProductRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<(List<Product> Items, int TotalCount)> GetAvailablePagedAsync(ProductQueryParameters query)
    {
   
        var products = _db.Products
            .AsNoTracking()               
            .Where(p => p.IsAvailable);

        if (!string.IsNullOrWhiteSpace(query.Category))
            products = products.Where(p => p.Category == query.Category);

        if (!string.IsNullOrWhiteSpace(query.Search))
            products = products.Where(p => p.Name.Contains(query.Search));   
        var totalCount = await products.CountAsync();

        var items = await products
            .OrderBy(p => p.Name)
            .ThenBy(p => p.Id)          
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();            

        return (items, totalCount);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);  
    }

    public async Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await _db.Products
            .AsNoTracking()
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
    }
}