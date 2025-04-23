namespace Demo.Elasticsearch.Services.Interfaces;

public interface IElasticsearchService<T>
{
    Task<T> GetByIdAsync(string id);

    Task<List<T>> GetAllAsync(int from = 0, int size = 10);

    Task<bool> IndexAsync(T document, string id);

    Task<bool> BulkIndexAsync(IEnumerable<(T document, string id)> documents);

    Task<bool> UpdateAsync(T document, string id);

    Task<bool> BulkUpdateAsync(IEnumerable<(T document, string id)> documents);

    Task<bool> DeleteAsync(string id);

    Task<bool> BulkDeleteAsync(IEnumerable<string> ids);
}
