namespace Demo.Elasticsearch.Services;

public interface IElasticService<T>
{
    Task IndexAsync(T document, string indexName);
    //Task UpdateAsync(string id, T document, string indexName);
    //Task DeleteAsync(string id, string indexName);
}
