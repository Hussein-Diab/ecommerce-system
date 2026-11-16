using System.ComponentModel.DataAnnotations;

namespace ECommerce.Client.Api.Dtos.Products;

public class ProductQueryParameters
{
    public string? Category { get; set; }
    public string? Search { get; set; }

    [Range(1, 500)]
    public int Page { get; set; } = 1;

    [Range(1, 50)]
    public int PageSize { get; set; } = 12;
}