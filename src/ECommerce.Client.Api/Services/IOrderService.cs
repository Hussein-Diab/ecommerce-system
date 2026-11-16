using ECommerce.Client.Api.Dtos.Orders;
using ECommerce.Client.Api.Services.Common;

namespace ECommerce.Client.Api.Services;

public interface IOrderService
{
    Task<ServiceResult<OrderDto>> PlaceOrderAsync(string customerId, PlaceOrderRequest request);
    Task<List<OrderDto>> GetMyOrdersAsync(string customerId);
    Task<ServiceResult<OrderDto>> GetOrderAsync(Guid orderId, string customerId);
    Task<ServiceResult<OrderDto>> CancelOrderAsync(Guid orderId, string customerId);
}