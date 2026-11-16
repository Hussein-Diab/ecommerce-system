using ECommerce.Client.Api.Dtos.Auth;
using ECommerce.Client.Api.Models.Entities;
using ECommerce.Client.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;

namespace ECommerce.Client.Api.Controllers;

public class AuthController : AppController
{
    private const string InvalidCredentialsMessage = "Invalid email or password.";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }
    private async Task<AuthResponse> BuildAuthResponseAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.CreateToken(user, roles);

        return new AuthResponse
        {
            Token = token.Token,
            ExpiresAtUtc = token.ExpiresAtUtc,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToList()
        };
    }
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || await _userManager.IsLockedOutAsync(user))
            return Unauthorized(new { message = InvalidCredentialsMessage });

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            await _userManager.AccessFailedAsync(user);
            return Unauthorized(new { message = InvalidCredentialsMessage });
        }

        await _userManager.ResetAccessFailedCountAsync(user);
        return Ok(await BuildAuthResponseAsync(user));
    }
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            Address = request.Address,
            PhoneNumber = request.PhoneNumber
        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {

            foreach (var error in result.Errors)
                ModelState.AddModelError(error.Code, error.Description);

            return ValidationProblem(ModelState);
        }
        var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return Problem("Could not assign the Customer role.");   // 500
        }
        return Ok(await BuildAuthResponseAsync(user));

    }
    [Authorize]   
    [HttpGet("me")]
    public async Task<ActionResult<ProfileResponse>> Me()
    {
        var user = await _userManager.FindByIdAsync(CurrentUserId);
        if (user is null)
            return Unauthorized();
        return Ok(new ProfileResponse
        {
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Address = user.Address,
            PhoneNumber = user.PhoneNumber ?? string.Empty
        });
    }
}