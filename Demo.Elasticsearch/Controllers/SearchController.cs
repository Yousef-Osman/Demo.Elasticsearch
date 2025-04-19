using Demo.Elasticsearch.Common;
using Demo.Elasticsearch.DTOs;
using Demo.Elasticsearch.Models;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Elasticsearch.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly ElasticsearchClient _elastic;

    public SearchController(ElasticsearchClient _elastic)
    {
        this._elastic = _elastic;
    }

    [HttpGet("{indexName}")]
    public async Task<IActionResult> Search(string indexName, [FromQuery] SearchRequestDto request)
    {
        return indexName.ToLower() switch
        {
            Constants.ProductsIndex => await SearchProductsAsync(request),
            _ => BadRequest($"Unknown document type: {indexName}")
        };
    }

    private async Task<IActionResult> SearchProductsAsync(SearchRequestDto request)
    {
        var descriptor = new SearchRequestDescriptor<Product>()
            .Indices(Constants.ProductsIndex)
            .From((request.PageNumber - 1) * request.PageSize)
            .Size(request.PageSize)
            .Query(q => q.Match(m => m.Field(f => f.Title).Query(request.Query)));
            //.Query(q => q.MultiMatch(m => m.Fields(f => new[] { f.Title, f.Description }).Query(request.Query)));

        if (!string.IsNullOrWhiteSpace(request.SortField))
        {
            descriptor.Sort(s => s
                .Field(request.SortField!, so => 
                    so.Order(request.SortOrder.ToLower() == "desc" ? SortOrder.Desc : SortOrder.Asc)));
        }

        var response = await _elastic.SearchAsync<Product>(descriptor);

        return HandleResponse(response);
    }

    private IActionResult HandleResponse<T>(SearchResponse<T> response) where T : class
    {
        if (!response.IsValidResponse)
        {
            return StatusCode(500, new
            {
                Error = "Search failed",
                Debug = response.DebugInformation
            });
        }

        return Ok(new
        {
            Results = response.Documents,
            Total = response.HitsMetadata?.Total ?? 0
        });
    }
}
