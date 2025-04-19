using Demo.Elasticsearch.Common;
using Demo.Elasticsearch.DTOs;
using Demo.Elasticsearch.Models;
using Demo.Elasticsearch.Services;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Elasticsearch.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IElasticService<Product> _elasticService;

    public ProductsController(IElasticService<Product> elasticService)
    {
        _elasticService = elasticService;
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(CreateProductDto input)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Title = input.Title,
            Description = input.Description,
            Category = input.Category,
            Brand = input.Brand,
            Price = input.Price,
            Stock = input.Stock,
        };

        await _elasticService.IndexAsync(product, Constants.ProductsIndex);

        return Ok();
    }
}
