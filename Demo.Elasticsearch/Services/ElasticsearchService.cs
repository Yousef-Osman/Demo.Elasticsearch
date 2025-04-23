using Demo.Elasticsearch.Services.Interfaces;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;

namespace Demo.Elasticsearch.Services;

public class ElasticsearchService<T> : IElasticsearchService<T> where T : class
{
    protected readonly ElasticsearchClient _client;
    protected readonly string _indexName;

    public ElasticsearchService(ElasticsearchClient client, string indexName)
    {
        _client = client;
        _indexName = indexName;
    }

    public async Task<T> GetByIdAsync(string id)
    {
        var response = await _client.GetAsync<T>(id, g => g.Index(_indexName));
        return response.IsValidResponse ? response.Source : null;
    }

    public async Task<List<T>> GetAllAsync(int from = 0, int size = 10)
    {
        var response = await _client.SearchAsync<T>(s => s
            .Index(_indexName)
            .From(from)
            .Size(size));

        //whithout validation check, it might contain incomplete results
        return response.IsValidResponse ? response.Documents.ToList() : new List<T>();
    }

    public async Task<bool> IndexAsync(T document, string id)
    {
        var response = await _client.IndexAsync(document, idx => idx.Index(_indexName).Id(id));
        return response.IsValidResponse;
    }

    public async Task<bool> BulkIndexAsync(IEnumerable<(T document, string id)> documents)
    {
        var bulkRequest = new BulkRequest(_indexName);

        foreach (var (document, id) in documents)
        {
            bulkRequest.Operations.Add(new BulkIndexOperation<T>(document) { Id = id });
        }

        var response = await _client.BulkAsync(bulkRequest);
        return response.IsValidResponse && !response.Errors;
    }

    public async Task<bool> UpdateAsync(T document, string id)
    {
        var response = await _client.UpdateAsync<T, T>(id, u => u.Index(_indexName).Doc(document));
        return response.IsValidResponse;
    }

    public async Task<bool> BulkUpdateAsync(IEnumerable<(T document, string id)> documents)
    {
        var bulkRequest = new BulkRequest(_indexName);

        foreach (var (document, id) in documents)
        {
            bulkRequest.Operations.Add(new BulkUpdateOperation<T, T>(id) 
            { 
                Doc = document
            });
        }

        var response = await _client.BulkAsync(bulkRequest);
        return response.IsValidResponse && !response.Errors;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var response = await _client.DeleteAsync<T>(id, d => d.Index(_indexName));
        return response.IsValidResponse;
    }

    public async Task<bool> BulkDeleteAsync(IEnumerable<string> ids)
    {
        var bulkRequest = new BulkRequest(_indexName);

        foreach (var id in ids)
        {
            bulkRequest.Operations.Add(new BulkDeleteOperation<T>(id));
        }

        var response = await _client.BulkAsync(bulkRequest);
        return response.IsValidResponse && !response.Errors;
    }
}
