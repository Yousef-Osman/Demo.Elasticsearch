using Demo.Elasticsearch.Common;
using Demo.Elasticsearch.DTOs;
using Demo.Elasticsearch.Models;
using Demo.Elasticsearch.Services.Interfaces;
using Elastic.Clients.Elasticsearch;

namespace Demo.Elasticsearch.Services;

public class ProductService : ElasticsearchService<Product>, IProductService
{
    public ProductService(ElasticsearchClient client, ILogger<ProductService> logger) : base(client, Constants.IndexNames.Products, logger) { }

    public async Task<SearchResult<Product>> SearchAsync(SearchRequestDto request)
    {
        var descriptor = new SearchRequestDescriptor<Product>()
            .Indices(Constants.IndexNames.Products)
            .From((request.PageNumber - 1) * request.PageSize)
            .Size(request.PageSize)
            //.Query(q => q.Match(m => m.Field(f => f.Name).Query(request.Query)));
            .Query(q => q
                .MultiMatch(mm => mm
                    .Fields(new[]
                    {
                        Infer.Field<Product>(p => p.Name),
                        Infer.Field<Product>(p => p.Description),
                    })
                    .Query(request.Query)
                )
            );

        var allowedSortFields = new[] { "name", "description" };

        if (!string.IsNullOrWhiteSpace(request.SortField))
        {
            if (!allowedSortFields.Contains(request.SortField))
                throw new ArgumentException($"Invalid sort field: {request.SortField}");

            descriptor.Sort(s => s
                .Field(new Field($"{request.SortField}.keyword"), so =>
                    so.Order(request.SortAsc ? SortOrder.Asc : SortOrder.Desc)));
        }

        var response = await _client.SearchAsync<Product>(descriptor);

        if (!response.IsValidResponse)
            throw new Exception($"Elasticsearch query failed: {response.DebugInformation}");

        return new SearchResult<Product>
        {
            Items = response.Documents.ToList(),
            Total = response.Total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
