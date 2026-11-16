using ECommerce.Client.Api.Dtos.Common;
using ECommerce.Client.Api.Dtos.Products;
using ECommerce.Client.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Client.Api.Controllers;

[AllowAnonymous]   // anyone can browse products, no login needed
public class ProductsController : AppController   // route: api/products
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET api/products?category=Electronics&search=mouse&page=1&pageSize=12
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetAll([FromQuery] ProductQueryParameters query)
    {
        return Ok(await _productService.GetProductsAsync(query));
    }

    // GET api/products/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var result = await _productService.GetProductAsync(id);

        if (!result.Success)
            return FailureResult(result);   

        return Ok(result.Data);
    }
}