using System.ComponentModel.DataAnnotations;
namespace ECommerce.Client.Api.Dtos.Auth;

public class RegisterRequest
{
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
    [Required]
    [MaxLength(300)]
    public string Address { get; set; } = string.Empty;
    [Required]
    [Phone]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;
}