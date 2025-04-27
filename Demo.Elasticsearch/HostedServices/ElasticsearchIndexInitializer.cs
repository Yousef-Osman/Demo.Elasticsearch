using Demo.Elasticsearch.Common;
using Demo.Elasticsearch.Models;
using Elastic.Clients.Elasticsearch;

namespace Demo.Elasticsearch.HostedServices;

public class ElasticsearchIndexInitializer : IHostedService
{
    private readonly ElasticsearchClient _elastic;
    private readonly ILogger<ElasticsearchIndexInitializer> _logger;

    public ElasticsearchIndexInitializer(ElasticsearchClient elastic,
        ILogger<ElasticsearchIndexInitializer> logger)
    {
        _elastic = elastic;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var tasks = new List<Task>
        {
            CreateIndexIfNotExistAsync<Product>(Constants.IndexNames.Products, cancellationToken),
            CreateIndexIfNotExistAsync<Category>(Constants.IndexNames.Categories, cancellationToken),
            CreateIndexIfNotExistAsync<Brand>(Constants.IndexNames.Brands, cancellationToken),
        };

        await Task.WhenAll(tasks);
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

        var create = await _elastic.Indices.CreateAsync(indexName, c => c
            .Settings(s => s
                .NumberOfShards(2)
                .NumberOfReplicas(1)
            ), ct);

        if (!create.IsValidResponse)
        {
            _logger.LogError("Failed to create index '{IndexName}'. Error: {Error}", indexName, create.ElasticsearchServerError?.ToString());
            throw new InvalidOperationException($"Failed to create index '{indexName}'. Server error: {create.ElasticsearchServerError?.Error?.Reason}");
        }

        _logger.LogInformation("Successfully created index '{IndexName}'", indexName);
    }
}
