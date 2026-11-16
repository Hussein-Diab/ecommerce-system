using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Client.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class AppController : ControllerBase
{
    
    protected string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException(
            "CurrentUserId was used in an endpoint without [Authorize].");
}