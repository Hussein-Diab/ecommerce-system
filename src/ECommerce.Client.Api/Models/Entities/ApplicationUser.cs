using Microsoft.AspNetCore.Identity;

namespace ECommerce.Client.Api.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}