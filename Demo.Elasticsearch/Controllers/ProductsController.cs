using Demo.Elasticsearch.DTOs.Product;
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
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var from = (pageNumber - 1) * pageSize;
        var products = await _productService.GetAllAsync(from, pageSize);

        return Ok(products);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Index(Product product)
    {
        var success = await _productService.IndexAsync(product, product.Id);

        if (!success)
            return BadRequest("Failed to create product");

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPost("bulk")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkIndex(IEnumerable<CreateProductDto> products)
    {
        var documents = products.Select(p => (p.Adapt<Product>(), p.Id));
        var success = await _productService.BulkIndexAsync(documents);

        if (!success)
            return BadRequest("Failed to bulk create products");

        return Accepted();
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, UpdateProductDto input)
    {
        var existingProduct = await _productService.GetByIdAsync(id);

        if (existingProduct == null)
            return NotFound();

        var product = input.Adapt<Product>();
        product.UpdatedAt = DateTime.UtcNow;

        var success = await _productService.UpdateAsync(product, id);

        if (!success)
            return BadRequest("Failed to update product");

        return NoContent();
    }

    [HttpPut("bulk")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkUpdate(IEnumerable<UpdateProductDto> input)
    {
        var products = input.Adapt<List<Product>>();

        products.ForEach(x => x.UpdatedAt = DateTime.UtcNow);

        var documents = products.Select(p => (p, p.Id));

        var success = await _productService.BulkUpdateAsync(documents);

        if (!success)
            return BadRequest("Failed to bulk update products");

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var existingProduct = await _productService.GetByIdAsync(id);

        if (existingProduct == null)
            return NotFound();

        var success = await _productService.DeleteAsync(id);

        if (!success)
            return BadRequest("Failed to delete product");

        return NoContent();
    }

    [HttpDelete("bulk")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkDelete(IEnumerable<string> ids)
    {
        var success = await _productService.BulkDeleteAsync(ids);

        if (!success)
            return BadRequest("Failed to bulk delete products");

        return NoContent();
    }
}
