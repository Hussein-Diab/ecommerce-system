using ECommerce.Client.Api.Data;              // ApplicationDbContext, DbSeeder
using ECommerce.Client.Api.Models.Entities;   // ApplicationUser
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

// ---- Controllers + OpenAPI ------------------------------------------
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Part C will add here: JWT settings, AddAuthentication().AddJwtBearer(...), AddAuthorization()
// Part D will add here: repositories and services (AddScoped<...>)

var app = builder.Build();

// =====================================================================
// 2. SEED THE DATABASE (roles + temporary products)
//    Runs once at startup, before the app accepts requests.
// =====================================================================
using (var scope = app.Services.CreateScope())
{
    // DbContext and RoleManager are SCOPED services: they must live inside
    // a scope. At startup there is no HTTP request (which normally creates
    // the scope), so we create one ourselves.
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

// Part C will add: app.UseAuthentication();   (must be BEFORE UseAuthorization)
app.UseAuthorization();

app.MapControllers();

app.Run();
