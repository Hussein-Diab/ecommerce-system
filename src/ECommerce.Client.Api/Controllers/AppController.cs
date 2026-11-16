using System.Security.Claims;
using ECommerce.Client.Api.Services.Common;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Client.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class AppController : ControllerBase
{
    // The id of the logged-in user, read from the JWT
    protected string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException(
            "CurrentUserId was used in an endpoint without [Authorize].");

    // Turns a FAILED ServiceResult into the right HTTP response
    protected ActionResult FailureResult<T>(ServiceResult<T> result) =>
        result.ErrorType switch
        {
            ServiceErrorType.NotFound => NotFound(new { message = result.Error }),    // 404
            _ => BadRequest(new { message = result.Error })                           // 400
        };
}