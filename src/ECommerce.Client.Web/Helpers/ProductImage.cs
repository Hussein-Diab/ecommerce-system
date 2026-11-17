namespace ECommerce.Client.Web.Helpers;

// One place for the "image or placeholder" rule, used by the product card,
// the product details page and the cart.
public static class ProductImage
{
    public const string Placeholder = "/images/placeholder.svg";

    // The product's image, or the placeholder when it has none
    public static string Url(string? imageUrl) =>
        string.IsNullOrWhiteSpace(imageUrl) ? Placeholder : imageUrl;
}
