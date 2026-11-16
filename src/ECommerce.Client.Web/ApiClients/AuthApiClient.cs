
using ECommerce.Client.Web.Models.Dtos;

namespace ECommerce.Client.Web.ApiClients;

public class AuthApiClient : IAuthApiClient
{
    private readonly HttpClient _http;   

    public AuthApiClient(HttpClient http)
    {
        _http = http;
    }

    public Task<ApiResult<AuthResponse>> LoginAsync(LoginRequest request) =>
        ApiResponseReader.SendAsync<AuthResponse>(() => _http.PostAsJsonAsync("api/auth/login", request));

    public Task<ApiResult<AuthResponse>> RegisterAsync(RegisterRequest request) =>
        ApiResponseReader.SendAsync<AuthResponse>(() => _http.PostAsJsonAsync("api/auth/register", request));

    public Task<ApiResult<ProfileResponse>> GetProfileAsync() =>
        ApiResponseReader.SendAsync<ProfileResponse>(() => _http.GetAsync("api/auth/me"));
}