using ECommerce.Client.Api.Models.Entities;

namespace ECommerce.Client.Api.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<Order?> GetByIdAsync(Guid id);
    Task<List<Order>> GetByCustomerAsync(string customerId);
    Task SaveChangesAsync();
}