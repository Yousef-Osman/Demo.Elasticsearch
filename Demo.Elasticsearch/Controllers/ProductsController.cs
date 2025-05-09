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

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var from = (pageNumber - 1) * pageSize;
        var result = await _productService.GetAllAsync(from, pageSize);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemDetails(HttpContext);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)] // Optional
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Index(Product product)
    {
        var result = await _productService.IndexAsync(product, product.Id);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = product.Id }, product)
            : result.ToProblemDetails(HttpContext);
    }

    [HttpPost("bulk")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BulkIndex(IEnumerable<CreateProductDto> products)
    {
        var documents = products.Select(p => (p.Adapt<Product>(), p.Id));
        var result = await _productService.BulkIndexAsync(documents);

        return result.IsSuccess
            ? Accepted()
            : BadRequest("Failed to bulk create products");
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(string id, UpdateProductDto input)
    {
        var existingProduct = await _productService.GetByIdAsync(id);

        if (existingProduct == null)
            return NotFound();

        var product = input.Adapt<Product>();
        product.UpdatedAt = DateTime.UtcNow;

        var result = await _productService.UpdateAsync(product, id);

        return result.IsSuccess
            ? NoContent()
            : BadRequest("Failed to update product");
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

        return result.IsSuccess
            ? NoContent()
            : BadRequest("Failed to bulk update products");
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(string id)
    {
        var existingProduct = await _productService.GetByIdAsync(id);

        if (existingProduct == null)
            return NotFound();

        var result = await _productService.DeleteAsync(id);

        return result.IsSuccess
            ? NoContent()
            : BadRequest("Failed to delete product");
    }

    [HttpDelete("bulk")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BulkDelete(IEnumerable<string> ids)
    {
        var result = await _productService.BulkDeleteAsync(ids);

        return result.IsSuccess
            ? NoContent()
            : BadRequest("Failed to bulk delete products");
    }
}
