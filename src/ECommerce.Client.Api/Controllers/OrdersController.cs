using ECommerce.Client.Api.Dtos.Orders;
using ECommerce.Client.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Client.Api.Controllers;

[Authorize(Roles = "Customer")]   // EVERY action here: valid token + Customer role
public class OrdersController : AppController   // route: api/orders
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // POST api/orders
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(PlaceOrderRequest request)
    {
        var result = await _orderService.PlaceOrderAsync(CurrentUserId, request);

        if (!result.Success)
            return FailureResult(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    // GET api/orders/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id)
    {
        var result = await _orderService.GetOrderAsync(id, CurrentUserId);

        if (!result.Success)
            return FailureResult(result);   // → 404 "Order not found."

        return Ok(result.Data);
    }

    // GET api/orders: the current customer's orders, newest first
    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetMine()
    {
        var orders = await _orderService.GetMyOrdersAsync(CurrentUserId);
        return Ok(orders);  
    }

    // POST api/orders/{id}/cancel
    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult> Cancel(Guid id)
    {
        var result = await _orderService.CancelOrderAsync(id, CurrentUserId);

        if (!result.Success)
            return FailureResult(result);  

        return NoContent();  
    }
}