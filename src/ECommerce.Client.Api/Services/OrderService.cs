using ECommerce.Client.Api.Dtos.Orders;
using ECommerce.Client.Api.Mappings;
using ECommerce.Client.Api.Models.Entities;
using ECommerce.Client.Api.Repositories;
using ECommerce.Client.Api.Services.Common;
using ECommerce.Contracts.Enums;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Client.Api.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        UserManager<ApplicationUser> userManager)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _userManager = userManager;
    }
    private static ServiceResult<OrderDto> Invalid(string message) =>
        ServiceResult<OrderDto>.Fail(ServiceErrorType.Validation, message);

    private static ServiceResult<OrderDto> NotFound(string message) =>
        ServiceResult<OrderDto>.Fail(ServiceErrorType.NotFound, message);
    public async Task<ServiceResult<OrderDto>> PlaceOrderAsync(string customerId, PlaceOrderRequest request)
    {

        var lines = request.Items.GroupBy(i => i.ProductId).Select(g => new { ProductId = g.Key, Quantity = g.Sum(i => i.Quantity) }).ToList();
        var products = await _productRepository.GetByIdsAsync(lines.Select(l => l.ProductId));
        var productsById = products.ToDictionary(p => p.Id);
        var items = new List<OrderItem>();

        foreach (var line in lines)
        {

            if (!productsById.TryGetValue(line.ProductId, out var product))
                return Invalid($"Product {line.ProductId} does not exist.");

            if (!product.IsAvailable)
                return Invalid($"{product.Name} is not available.");

            if (line.Quantity > product.StockQuantity)
                return Invalid($"Only {product.StockQuantity} of {product.Name} in stock.");

            items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = line.Quantity
            });
        }
        var customer = await _userManager.FindByIdAsync(customerId);
        if (customer is null)
            return NotFound("Customer not found.");
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            CustomerName = customer.FullName,
            ShippingAddress = request.ShippingAddress,
            PhoneNumber = request.PhoneNumber,
            Status = OrderStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow,
            IsPaid = false,
            Items = items,
            TotalAmount = items.Sum(i => i.LineTotal)
        };
        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        return ServiceResult<OrderDto>.Ok(order.ToDto());
    }
    private async Task<Order?> LoadOwnedOrderAsync(Guid orderId, string customerId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order is null || order.CustomerId != customerId)
            return null;

        return order;
    }
    public async Task<List<OrderDto>> GetMyOrdersAsync(string customerId)
    {
        var orders = await _orderRepository.GetByCustomerAsync(customerId);
        List<OrderDto> MyOrders = new();
        foreach (var o in orders)
        {
            MyOrders.Add(o.ToDto());
        }
        return MyOrders;
    }
    public async Task<ServiceResult<OrderDto>> GetOrderAsync(Guid orderId, string customerId)
    {
        var order = await LoadOwnedOrderAsync(orderId, customerId);
        if (order is null)
        {
            return NotFound("Order Not Found");
        }
        else
        {
            return ServiceResult<OrderDto>.Ok(order.ToDto());
        }
    }
    public async Task<ServiceResult<OrderDto>> CancelOrderAsync(Guid orderId, string customerId)
    {
        var order = await LoadOwnedOrderAsync(orderId, customerId);
        if (order is null)
        {
            return NotFound("Order Not Found");
        }
        else if (order.Status != OrderStatus.Pending)
        {
            return Invalid("Only pending orders can be cancelled.");
        }


        order.Status = OrderStatus.Cancelled;
        await _orderRepository.SaveChangesAsync();
        //day 4 publsih ordercancelled here
        return ServiceResult<OrderDto>.Ok(order.ToDto());

    }
}