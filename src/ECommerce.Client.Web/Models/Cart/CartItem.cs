namespace ECommerce.Client.Web.Models.Cart;

// One line of the cart. Stored in the SESSION as JSON, never in a database.
public class CartItem
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }          // DISPLAY ONLY: the API recalculates every price at checkout
    public string? ImageUrl { get; set; }       // for the thumbnail in the cart
    public int Quantity { get; set; }

    // Computed, never stored: it can't disagree with Price × Quantity
    public decimal LineTotal => Price * Quantity;
}
