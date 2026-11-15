using ECommerce.Client.Api.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Client.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        if (!await roleManager.RoleExistsAsync("Customer"))
        {
            await roleManager.CreateAsync(new IdentityRole("Customer"));
        }
        // Ask the container for the database
        var db = services.GetRequiredService<ApplicationDbContext>();

        // TEMPORARY: removed on Day 4 when products come from Admin via RabbitMQ
        if (!await db.Products.AnyAsync())   // table is empty?
        {
            var products = new List<Product>
    {
        new Product
        {
            Id = 1,
            Name = "Wireless Mouse",
            Description = "Ergonomic 2.4GHz wireless mouse",
            Price = 19.99m,
            Category = "Computer Accessories",
            StockQuantity = 50,
            IsAvailable = true,
            LastUpdatedUtc = DateTime.UtcNow
        },
          new Product
        {
            Id = 2,
            Name = "Gaming Laptop",
            Description = "i7 11th gen",
            Price = 799.99m,
            Category = "Electronics",
            StockQuantity = 5,
            IsAvailable = true,
            LastUpdatedUtc = DateTime.UtcNow
        },
           new Product
        {
            Id = 3,
            Name = "Gaming Monitor 122Hz",
            Description = "LG",
            Price = 99.99m,
            Category = "Monitors",
            StockQuantity = 0,
            IsAvailable = true,
            LastUpdatedUtc = DateTime.UtcNow
        }
    };

            db.Products.AddRange(products);
            await db.SaveChangesAsync();   // NOW the rows are actually inserted
        }
    }
}