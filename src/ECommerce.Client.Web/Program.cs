using ECommerce.Client.Web.ApiClients;
using ECommerce.Client.Web.Handlers;
using ECommerce.Client.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
    ?? throw new InvalidOperationException("ApiSettings:BaseUrl is missing in appsettings.json.");

// MVC + anti-forgery check on EVERY POST automatically (no forgotten [ValidateAntiForgeryToken])
builder.Services.AddControllersWithViews(options =>
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

// ---- Login cookie (browser ↔ Web) ----------------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";              // [Authorize] redirects here when not logged in
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied"; // logged in but wrong role
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);  // same as the JWT lifetime
        options.SlidingExpiration = false;                  // the JWT doesn't slide, so the cookie mustn't either
        options.Cookie.HttpOnly = true;                     // JavaScript can't read it (protects against XSS theft)
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;   // only sent over HTTPS
    });
builder.Services.AddAuthorization();

// ---- Session (stores the cart) -------------------------------------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ---- Calling the API (Web ↔ API) -----------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<BearerTokenHandler>();

builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddHttpClient<IProductApiClient, ProductApiClient>(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<BearerTokenHandler>();
builder.Services.AddHttpClient<IOrderApiClient, OrderApiClient>(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<BearerTokenHandler>();

// ---- Cart (stored in the session) ----------------------------------
builder.Services.AddScoped<ICartService, CartService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();          // after UseRouting, before the endpoints
app.UseAuthentication();   // reads the cookie → fills User
app.UseAuthorization();    // checks [Authorize]

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}")   // the shop is the home page
    .WithStaticAssets();

app.Run();