using Demo.Elasticsearch.Common;
using Demo.Elasticsearch.Models;
using Elastic.Clients.Elasticsearch;

namespace Demo.Elasticsearch.HostedServices;

public class ElasticsearchIndexInitializer : IHostedService
{
    private readonly ElasticsearchClient _elastic;

    public ElasticsearchIndexInitializer(ElasticsearchClient elastic)
    {
        _elastic = elastic;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await CreateIndexIfNotExistAsync<Product>(Constants.IndexNames.Products, cancellationToken);
        await CreateIndexIfNotExistAsync<Category>(Constants.IndexNames.Categories, cancellationToken);
        await CreateIndexIfNotExistAsync<Brand>(Constants.IndexNames.Brands, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task CreateIndexIfNotExistAsync<T>(string indexName, CancellationToken ct) where T : class
    {
        var exists = await _elastic.Indices.ExistsAsync(indexName, ct);

        if (exists.Exists)
            return;

        var create = await _elastic.Indices.CreateAsync(indexName, ct);

        if (!create.IsValidResponse)
            throw new Exception($"Failed to create index {indexName}");
    }
}
