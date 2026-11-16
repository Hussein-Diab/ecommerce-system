using ECommerce.Client.Web.Models.Dtos;

namespace ECommerce.Client.Web.ApiClients;

public interface IOrderApiClient
{
    Task<ApiResult<OrderDto>> PlaceOrderAsync(PlaceOrderRequest request);
    Task<ApiResult<List<OrderDto>>> GetMyOrdersAsync();
    Task<ApiResult<OrderDto>> GetOrderAsync(Guid id);
    Task<ApiResult<object>> CancelOrderAsync(Guid id);
}