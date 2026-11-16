using ECommerce.Client.Api.Models.Entities;

namespace ECommerce.Client.Api.Services;

public interface ITokenService
{
    TokenResult CreateToken(ApplicationUser user, IList<string> roles);
}