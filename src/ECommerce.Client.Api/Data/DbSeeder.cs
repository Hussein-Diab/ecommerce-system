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

        var db = services.GetRequiredService<ApplicationDbContext>();

        // TEMPORARY: removed on Day 4 when products come from Admin via RabbitMQ.
        // Only runs when the table is empty. To re-seed: DELETE FROM products; then restart the API.
        // Image files live in ECommerce.Client.Web/wwwroot/images/products/ (names must match exactly).
        if (!await db.Products.AnyAsync())
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Wireless Mouse",
                    Description = "Ergonomic 2.4GHz wireless mouse with a silent click and 18-month battery life.",
                    Price = 19.99m,
                    Category = "Computer Accessories",
                    StockQuantity = 50,
                    IsAvailable = true,
                    ImageUrl = "/images/products/wireless-mouse.jpg",
                    LastUpdatedUtc = DateTime.UtcNow
                },
                new Product
                {
                    Id = 2,
                    Name = "Gaming Laptop",
                    Description = "15.6\" gaming laptop, Intel i7 11th gen, 16 GB RAM, RTX graphics.",
                    Price = 799.99m,
                    Category = "Electronics",
                    StockQuantity = 5,                     // low stock: tests "Only 5 left" and the stock limit
                    IsAvailable = true,
                    ImageUrl = "/images/products/gaming-laptop.jpg",
                    LastUpdatedUtc = DateTime.UtcNow
                },
                new Product
                {
                    Id = 3,
                    Name = "Gaming Monitor 144Hz",
                    Description = "27\" LG IPS monitor, 144Hz refresh rate, 1ms response time.",
                    Price = 99.99m,
                    Category = "Monitors",
                    StockQuantity = 0,                     // out of stock: tests "Out of stock"
                    IsAvailable = true,
                    ImageUrl = "/images/products/gaming-monitor.jpg",
                    LastUpdatedUtc = DateTime.UtcNow
                },
                new Product
                {
                    Id = 4,
                    Name = "Mechanical Keyboard",
                    Description = "RGB mechanical keyboard with hot-swappable brown switches.",
                    Price = 59.99m,
                    Category = "Computer Accessories",     // same category as the mouse: tests the filter
                    StockQuantity = 30,
                    IsAvailable = true,
                    ImageUrl = "/images/products/mechanical-keyboard.jpg",
                    LastUpdatedUtc = DateTime.UtcNow
                },
                new Product
                {
                    Id = 5,
                    Name = "USB-C Hub",
                    Description = "7-in-1 USB-C hub: HDMI 4K, 3x USB 3.0, SD card reader, 100W charging.",
                    Price = 34.50m,
                    Category = "Electronics",
                    StockQuantity = 20,
                    IsAvailable = true,
                    ImageUrl = null,                       // no image: tests the placeholder
                    LastUpdatedUtc = DateTime.UtcNow
                },
                new Product
                {
                    Id = 6,
                    Name = "Noise-Cancelling Headphones",
                    Description = "Over-ear Bluetooth headphones with active noise cancelling.",
                    Price = 149.00m,
                    Category = "Audio",
                    StockQuantity = 10,
                    IsAvailable = false,                   // unavailable: hidden from the list, can't be ordered
                    ImageUrl = "/images/products/headphones.jpg",
                    LastUpdatedUtc = DateTime.UtcNow
                }
            };

            db.Products.AddRange(products);
            await db.SaveChangesAsync();
        }
    }
}
