using Demo.Elasticsearch.DTOs;
using Demo.Elasticsearch.Models;
using Demo.Elasticsearch.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Elasticsearch.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(CreateProductDto input)
    {
        var product = new Product
        {
            Id = input.Id,
            Name = input.Name,
            Description = input.Description,
            Category = input.Category,
            Brand = input.Brand,
            Price = input.Price,
            Stock = input.Stock,
        };

        var result = await _productService.IndexAsync(product, product.Id);

        return result ? Created() : StatusCode(StatusCodes.Status500InternalServerError);
    }
}
