using Demo.Elasticsearch.Common;

namespace Demo.Elasticsearch.Services.Interfaces;

public interface IElasticsearchService<T>
{
    Task<Result<T>> GetByIdAsync(string id);

    Task<Result<List<T>>> GetAllAsync(int from, int size);

    Task<Result> IndexAsync(T document, string id);

    Task<Result> BulkIndexAsync(IEnumerable<(T document, string id)> documents);

    Task<Result> UpdateAsync(T document, string id);

    Task<Result> BulkUpdateAsync(IEnumerable<(T document, string id)> documents);

    Task<Result> DeleteAsync(string id);

    Task<Result> BulkDeleteAsync(IEnumerable<string> ids);
}
