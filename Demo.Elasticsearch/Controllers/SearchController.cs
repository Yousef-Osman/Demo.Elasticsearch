using Demo.Elasticsearch.DTOs;
using Demo.Elasticsearch.Extensions;
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
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchProducts([FromQuery] SearchRequestDto request)
    {
        var result = await _productSearchService.SearchAsync(request);

        return result.Match<IActionResult>(
            value => Ok(value),
            _ => result.ToProblemDetails(HttpContext)
        );
    }
}
