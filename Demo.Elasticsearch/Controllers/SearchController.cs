using Demo.Elasticsearch.DTOs;
using Demo.Elasticsearch.Models;
using Demo.Elasticsearch.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Elasticsearch.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly ISearchService<Product> _productSearchService;

    public SearchController(IProductService productService)
    {
        _productSearchService = productService;
    }

    [HttpGet("Products")]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchProducts([FromQuery] SearchRequestDto request)
    {
        SearchResult<Product> result;

        try
        {
            result = await _productSearchService.SearchAsync(request);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }

        return Ok(result);
    }
}
