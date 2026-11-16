using ECommerce.Client.Api.Data;
using ECommerce.Client.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Client.Api.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _db;

    public OrderRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Order order)
    {
        await _db.Orders.AddAsync(order);
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {

        return await _db.Orders
            .Include(o => o.Items)         
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<Order>> GetByCustomerAsync(string customerId)
    {
        return await _db.Orders
            .AsNoTracking()              
            .Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)  
            .OrderByDescending(o => o.CreatedAtUtc)   
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}