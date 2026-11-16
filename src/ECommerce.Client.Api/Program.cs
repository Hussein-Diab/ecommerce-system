using ECommerce.Client.Api.Data;              // ApplicationDbContext, DbSeeder
using ECommerce.Client.Api.Models.Entities;   // ApplicationUser
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ECommerce.Client.Api.Settings;
using System.Security.Claims;                          // ClaimTypes
using System.Text;                                      // Encoding
using Microsoft.AspNetCore.Authentication.JwtBearer;    // JwtBearerDefaults
using Microsoft.IdentityModel.Tokens;
using ECommerce.Client.Api.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString));
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = true;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();



builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Jwt section is missing in configuration.");


if (string.IsNullOrWhiteSpace(jwtSettings.Key) || jwtSettings.Key.Length < 32)
{
    throw new InvalidOperationException("Jwt:Key is missing or shorter than 32 characters. Set it with dotnet user-secrets.");
}
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)   // "Bearer" is the default way to identify users
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {

            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,


            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,


            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddControllers();

// Part D will add here: repositories and services (AddScoped<...>)

var app = builder.Build();

// =====================================================================
// 2. SEED THE DATABASE (roles + temporary products)
//    Runs once at startup, before the app accepts requests.
// =====================================================================
using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

// =====================================================================
// 3. HTTP PIPELINE (middleware). ORDER MATTERS: each request passes
//    through these in this order.
// =====================================================================
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();   // https://localhost:7102/openapi/v1.json, dev only
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();   

app.MapControllers();

app.Run();
