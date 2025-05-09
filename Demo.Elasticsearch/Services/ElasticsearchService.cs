using Demo.Elasticsearch.Common;
using Demo.Elasticsearch.Common.Errors;
using Demo.Elasticsearch.Services.Interfaces;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Result = Demo.Elasticsearch.Common.Result;

namespace Demo.Elasticsearch.Services;

public class ElasticsearchService<T> : IElasticsearchService<T> where T : class
{
    protected readonly ElasticsearchClient _client;
    protected readonly string _indexName;
    private readonly ILogger<ElasticsearchService<T>> _logger;

    public ElasticsearchService(ElasticsearchClient client,
        string indexName,
        ILogger<ElasticsearchService<T>> logger)
    {
        _client = client;
        _indexName = indexName;
        _logger = logger;
    }

    public virtual async Task<Result<T>> GetByIdAsync(string id)
    {
        var response = await _client.GetAsync<T>(id, idx => idx.Index(_indexName));

        if (!response.IsValidResponse)
        {
            var errorMessage = response.ApiCallDetails.OriginalException?.Message;
            var statusCode = response.ApiCallDetails.HttpStatusCode;

            _logger.LogError("Error getting document with ID {Id}. Status code: {StatusCode}, Error: {Error}", id, statusCode, errorMessage);

            if (statusCode == StatusCodes.Status404NotFound)
                return Result<T>.Failure(DocumentErrors.NotFound());

            return Result<T>.Failure(DocumentErrors.ServerError());
        }

        if (!response.Found)
            return Result<T>.Failure(DocumentErrors.NotFound());

        return Result<T>.Success(response.Source);
    }

    public virtual async Task<Result<List<T>>> GetAllAsync(int from, int size)
    {
        var searchRequest = new SearchRequest<T>(_indexName)
        {
            From = from,
            Size = size,
            Query = new MatchAllQuery(),
        };

        var response = await _client.SearchAsync<T>(searchRequest);

        if (!response.IsValidResponse)
        {
            var errorMessage = response.ApiCallDetails.OriginalException?.Message;
            var statusCode = response.ApiCallDetails.HttpStatusCode;

            _logger.LogError("Error getting documents. Status code: {StatusCode}, Error: {Error}", statusCode, errorMessage);

            return Result<List<T>>.Failure(DocumentErrors.ServerError());
        }

        return Result<List<T>>.Success(response.Documents.ToList());
    }

    public virtual async Task<Result> IndexAsync(T document, string id)
    {
        var response = await _client.IndexAsync(document, idx => idx.Index(_indexName).Id(id));

        if (!response.IsValidResponse)
        {
            var errorMessage = response.ApiCallDetails.OriginalException?.Message;
            var statusCode = response.ApiCallDetails.HttpStatusCode;

            _logger.LogError("Error indexing document with ID {Id}. Status code: {StatusCode}, Error: {Error}", id, statusCode, errorMessage);

            return Result.Failure(DocumentErrors.ServerError());
        }

        return Result.Success();
    }

    public virtual async Task<Result> BulkIndexAsync(IEnumerable<(T document, string id)> documents)
    {
        var bulkRequest = new BulkRequest(_indexName) { Operations = [] };

        foreach (var (document, id) in documents)
        {
            bulkRequest.Operations.Add(new BulkIndexOperation<T>(document) { Id = id });
        }

        var response = await _client.BulkAsync(bulkRequest);

        if (!response.IsValidResponse)
        {
            var errorMessage = response.ApiCallDetails.OriginalException?.Message;
            var statusCode = response.ApiCallDetails.HttpStatusCode;

            _logger.LogError("Error bulk indexing documents. Status code: {StatusCode}, Error: {Error}", statusCode, errorMessage);

            return Result.Failure(DocumentErrors.ServerError());
        }

        if (response.Errors)
        {
            _logger.LogError("Bulk indexing had errors: {ErrorItems}",
                        string.Join(", ", response.ItemsWithErrors.Select(i => i.Error.ToString())));

            return Result.Failure(DocumentErrors.ServerError());
        }

        return Result.Success();
    }

    public virtual async Task<Result> UpdateAsync(T document, string id)
    {
        var response = await _client.UpdateAsync<T, T>(id, u => u
            .Index(_indexName)
            .Doc(document)
            .RetryOnConflict(3));

        if (!response.IsValidResponse)
        {
            var errorMessage = response.ApiCallDetails.OriginalException?.Message;
            var statusCode = response.ApiCallDetails.HttpStatusCode;

            _logger.LogError("Error updating document with ID {Id}. Status code: {StatusCode}, Error: {Error}", id, statusCode, errorMessage);

            if (statusCode == StatusCodes.Status404NotFound)
                return Result.Failure(DocumentErrors.NotFound());

            return Result.Failure(DocumentErrors.ServerError());
        }

        return Result.Success();
    }

    public virtual async Task<Result> BulkUpdateAsync(IEnumerable<(T document, string id)> documents)
    {
        var bulkRequest = new BulkRequest(_indexName) { Operations = [] };

        foreach (var (document, id) in documents)
        {
            bulkRequest.Operations.Add(new BulkUpdateOperation<T, T>(id)
            {
                Doc = document
            });
        }

        var response = await _client.BulkAsync(bulkRequest);

        if (!response.IsValidResponse)
        {
            var errorMessage = response.ApiCallDetails.OriginalException?.Message;
            var statusCode = response.ApiCallDetails.HttpStatusCode;

            _logger.LogError("Error bulk updating documents. Status code: {StatusCode}, Error: {Error}", statusCode, errorMessage);

            return Result.Failure(DocumentErrors.ServerError());
        }

        if (response.Errors)
        {
            _logger.LogError("Bulk update had errors: {ErrorItems}",
                        string.Join(", ", response.ItemsWithErrors.Select(i => i.Error.ToString())));

            return Result.Failure(DocumentErrors.ServerError());
        }

        return Result.Success();
    }

    public virtual async Task<Result> DeleteAsync(string id)
    {
        var response = await _client.DeleteAsync<T>(id, d => d.Index(_indexName));

        if (!response.IsValidResponse)
        {
            var errorMessage = response.ApiCallDetails.OriginalException?.Message;
            var statusCode = response.ApiCallDetails.HttpStatusCode;

            _logger.LogError("Error deleting document with ID {Id}. Status code: {StatusCode}, Error: {Error}", id, statusCode, errorMessage);

            if (statusCode == StatusCodes.Status404NotFound)
                return Result.Failure(DocumentErrors.NotFound());

            return Result.Failure(DocumentErrors.ServerError());
        }

        return Result.Success();
    }

    public virtual async Task<Result> BulkDeleteAsync(IEnumerable<string> ids)
    {
        var bulkRequest = new BulkRequest() { Operations = [] };

        foreach (var id in ids)
        {
            bulkRequest.Operations.Add(new BulkDeleteOperation<T>(id) { Index = _indexName });
        }

        var response = await _client.BulkAsync(bulkRequest);

        if (!response.IsValidResponse)
        {
            var errorMessage = response.ApiCallDetails.OriginalException?.Message;
            var statusCode = response.ApiCallDetails.HttpStatusCode;

            _logger.LogError("Error bulk deleting documents. Status code: {StatusCode}, Error: {Error}", statusCode, errorMessage);

            return Result.Failure(DocumentErrors.ServerError());
        }

        if (response.Errors)
        {
            _logger.LogError("Bulk delete had errors: {ErrorItems}",
                        string.Join(", ", response.ItemsWithErrors.Select(i => i.Error.ToString())));

            return Result.Failure(DocumentErrors.ServerError());
        }

        return Result.Success();
    }
}
