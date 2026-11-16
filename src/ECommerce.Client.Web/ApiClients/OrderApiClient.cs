using ECommerce.Client.Web.Models.Dtos;

namespace ECommerce.Client.Web.ApiClients;

public class OrderApiClient : IOrderApiClient
{
    private readonly HttpClient _http; 

    public OrderApiClient(HttpClient http)
    {
        _http = http;
    }


    public Task<ApiResult<OrderDto>> PlaceOrderAsync(PlaceOrderRequest request)
    {
        return ApiResponseReader.SendAsync<OrderDto>(
            () => _http.PostAsJsonAsync("api/orders", request));   
    }
    public Task<ApiResult<List<OrderDto>>> GetMyOrdersAsync()
    {
        return ApiResponseReader.SendAsync<List<OrderDto>>(
            () => _http.GetAsync("api/orders"));
    }
    public Task<ApiResult<OrderDto>> GetOrderAsync(Guid id)
    {
        return ApiResponseReader.SendAsync<OrderDto>(
            () => _http.GetAsync($"api/orders/{id}"));
    }
    public Task<ApiResult<object>> CancelOrderAsync(Guid id)
    {
        return ApiResponseReader.SendAsync<object>(
            () => _http.PostAsync($"api/orders/{id}/cancel", null));  
    }
}