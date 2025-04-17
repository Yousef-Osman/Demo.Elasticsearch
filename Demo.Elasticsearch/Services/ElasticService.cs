using Elastic.Clients.Elasticsearch;

namespace Demo.Elasticsearch.Services;

public class ElasticService: IElasticService
{
    private readonly ElasticsearchClient _client;

    public ElasticService(ElasticsearchClient client)
    {
        _client = client;
    }
}
