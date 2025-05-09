using Demo.Elasticsearch.DTOs.Product;
using Demo.Elasticsearch.Extensions;
using Demo.Elasticsearch.Models;
using Demo.Elasticsearch.Services.Interfaces;
using Mapster;
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

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _productService.GetByIdAsync(id);

        return result.Match<IActionResult>(
            value => Ok(value),
            _ => result.ToProblemDetails(HttpContext)
        );
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var from = (pageNumber - 1) * pageSize;
        var result = await _productService.GetAllAsync(from, pageSize);

        return result.Match<IActionResult>(
            value => Ok(value),
            _ => result.ToProblemDetails(HttpContext)
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)] // Optional
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Index(Product product)
    {
        var result = await _productService.IndexAsync(product, product.Id);

        return result.Match<IActionResult>(
            () => CreatedAtAction(nameof(GetById), new { id = product.Id }, product),
            _ => result.ToProblemDetails(HttpContext)
        );
    }

    [HttpPost("bulk")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BulkIndex(IEnumerable<CreateProductDto> products)
    {
        var documents = products.Select(p => (p.Adapt<Product>(), p.Id));
        var result = await _productService.BulkIndexAsync(documents);

        return result.Match<IActionResult>(
            () => Accepted(),
            _ => result.ToProblemDetails(HttpContext)
        );
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(string id, UpdateProductDto input)
    {
        var productResult = await _productService.GetByIdAsync(id);

        if (!productResult.IsSuccess)
            return productResult.ToProblemDetails(HttpContext);

        var product = input.Adapt<Product>();
        product.UpdatedAt = DateTime.UtcNow;

        var result = await _productService.UpdateAsync(product, id);

        return result.Match<IActionResult>(
            () => NoContent(),
            _ => result.ToProblemDetails(HttpContext)
        );
    }

    [HttpPut("bulk")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BulkUpdate(IEnumerable<UpdateProductDto> input)
    {
        var products = input.Adapt<List<Product>>();

        products.ForEach(x => x.UpdatedAt = DateTime.UtcNow);

        var documents = products.Select(p => (p, p.Id));

        var result = await _productService.BulkUpdateAsync(documents);

        return result.Match<IActionResult>(
            () => NoContent(),
            _ => result.ToProblemDetails(HttpContext)
        );
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(string id)
    {
        var productResult = await _productService.GetByIdAsync(id);

        if (!productResult.IsSuccess)
            return productResult.ToProblemDetails(HttpContext);

        var result = await _productService.DeleteAsync(id);

        return result.Match<IActionResult>(
            () => NoContent(),
            _ => result.ToProblemDetails(HttpContext)
        );
    }

    [HttpDelete("bulk")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BulkDelete(IEnumerable<string> ids)
    {
        var result = await _productService.BulkDeleteAsync(ids);

        return result.Match<IActionResult>(
            () => NoContent(),
            _ => result.ToProblemDetails(HttpContext)
        );
    }
}
