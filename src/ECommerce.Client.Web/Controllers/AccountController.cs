using System.Security.Claims;
using ECommerce.Client.Web.ApiClients;
using ECommerce.Client.Web.Models.Dtos;
using ECommerce.Client.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Client.Web.Controllers;

public class AccountController : Controller   
{
    private readonly IAuthApiClient _authApi;

    public AccountController(IAuthApiClient authApi)
    {
        _authApi = authApi;
    }
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Products");

        ViewData["ReturnUrl"] = returnUrl; 
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authApi.LoginAsync(new LoginRequest
        {
            Email = model.Email,
            Password = model.Password
        });

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        await SignInWithTokenAsync(result.Data!, model.RememberMe);
        TempData["Success"] = $"Welcome back, {result.Data!.FullName}!";

        return RedirectToLocal(returnUrl);
    }
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Products");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var result = await _authApi.RegisterAsync(new RegisterRequest
        {
            FullName = model.FullName,
            Email = model.Email,
            Password = model.Password,
            Address = model.Address,
            PhoneNumber = model.PhoneNumber
        });

        if (!result.Success)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            return View(model);
        }
        await SignInWithTokenAsync(result.Data!, rememberMe: false);
        TempData["Success"] = $"Welcome, {result.Data!.FullName}! Your account was created.";

        return RedirectToAction("Index", "Products");
    }
    [HttpPost]                     
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);  
        HttpContext.Session.Clear();                                                          

        TempData["Success"] = "You have been logged out.";
        return RedirectToAction("Index", "Products");
    }


    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
    private async Task SignInWithTokenAsync(AuthResponse auth, bool rememberMe)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, auth.UserId),
            new(ClaimTypes.Name, auth.FullName),     
            new(ClaimTypes.Email, auth.Email)
        };
        claims.AddRange(auth.Roles.Select(role => new Claim(ClaimTypes.Role, role)));  

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var properties = new AuthenticationProperties
        {
            IsPersistent = rememberMe,        
            ExpiresUtc = auth.ExpiresAtUtc  
        };

        properties.StoreTokens(new[]
        {
            new AuthenticationToken { Name = "access_token", Value = auth.Token }
        });

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            properties);
    }

   
    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Products");
    }
}