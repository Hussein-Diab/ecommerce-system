using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ECommerce.Client.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
namespace ECommerce.Client.Api.Data;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).ValueGeneratedNever();   // Id comes from the Admin, not auto-generated
            e.Property(p => p.Name).IsRequired().HasMaxLength(200);
            e.Property(p => p.Price).HasPrecision(18, 2);
            e.Property(p => p.Category).HasMaxLength(100);
            e.Property(p => p.Description).HasMaxLength(1000);

        });
        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.TotalAmount).HasPrecision(18, 2);
            e.Property(o => o.CustomerId).HasMaxLength(255);
            e.Property(o => o.CustomerName).IsRequired().HasMaxLength(200);
            e.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);   // saves "Pending" instead of 0
            e.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(300);
            e.HasMany(o => o.Items)
             .WithOne(i => i.Order)
             .HasForeignKey(i => i.OrderId)
             .OnDelete(DeleteBehavior.Cascade);
            e.Property(o => o.PhoneNumber).HasMaxLength(20);
            e.Property(o => o.StatusNote).HasMaxLength(500);
            e.Property(o => o.TrackingNumber).HasMaxLength(100);
            e.HasIndex(o => o.CustomerId);
        });
        modelBuilder.Entity<OrderItem>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.UnitPrice).HasPrecision(18, 2);
            e.Property(i => i.ProductName).HasMaxLength(200);
            e.Ignore(i => i.LineTotal);                          // computed, not saved
        });
    }
}