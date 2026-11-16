
using ECommerce.Client.Web.Models.Dtos;

namespace ECommerce.Client.Web.ApiClients;

public interface IAuthApiClient
{
    Task<ApiResult<AuthResponse>> LoginAsync(LoginRequest request);
    Task<ApiResult<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<ApiResult<ProfileResponse>> GetProfileAsync();
}