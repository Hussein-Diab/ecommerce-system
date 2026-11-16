using System.ComponentModel.DataAnnotations;
namespace ECommerce.Client.Api.Dtos.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new ();
    public DateTime ExpiresAtUtc { get; set; }
}