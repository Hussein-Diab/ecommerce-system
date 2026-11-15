namespace ECommerce.Contracts.Constants;
public static class RoutingKeys

{
    public static class Exchanges
    {
        // A single topic exchange handles both Client -> Admin and Admin -> Client directions
        public const string Default = "app.events";
    }
    public static class Keys
    {
        //client to admin
        public const string OrderPlaced = "order.placed";
        public const string OrderCancelled = "order.cancelled";
        public const string PaymentCompleted = "payment.completed";
        public const string ReviewSubmitted = "review.submitted";

        //admin to client
        public const string OrderStatusChanged = "order.status.changed";
        public const string ProductChanged = "product.changed";
    }
    public static class Queues
    {
        // Admin queues (Client -> Admin)
        public const string AdminOrders = "admin.orders";
        public const string AdminPayments = "admin.payments";
        public const string AdminReviews = "admin.reviews";

        // Client queues (Admin -> Client)
        public const string ClientOrderUpdates = "client.order-updates";
        public const string ClientProducts = "client.products";
    }
}