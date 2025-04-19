using Elastic.Clients.Elasticsearch;

namespace Demo.Elasticsearch.Services;

public class ElasticService<T>: IElasticService<T> where T : class
{
    private readonly ElasticsearchClient _elastic;

    public ElasticService(ElasticsearchClient elastic)
    {
        _elastic = elastic;
    }

    public async Task IndexAsync(T document, string indexName)
    {
        var response = await _elastic
            .IndexAsync(document, idx => idx.Index(indexName).OpType(OpType.Index));

        if (!response.IsValidResponse)
            throw new Exception(response.ElasticsearchServerError?.ToString());
    }

    //public async Task<List<T>> SearchAsync(string indexName, string query)
    //{
    //    var response = await _elastic.SearchAsync<T>(s => s
    //        .Indices(indexName)
    //        .Query(q => q.Match(m => m.Field("_all").Query(query)))
    //    );

    //    if (!response.IsValidResponse)
    //    {
    //        //_logger.LogError("Search failed: {Reason}", response.DebugInformation);
    //        return new List<T>();
    //    }

    //    return response.Documents.ToList();
    //}
}
