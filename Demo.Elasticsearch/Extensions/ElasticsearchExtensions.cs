using Demo.Elasticsearch.Configuration;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Options;

namespace Demo.Elasticsearch.Extensions;

public static class ElasticsearchExtensions
{
    public static IServiceCollection AddElasticsearch(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var config = sp.GetRequiredService<IOptions<ElasticSettings>>().Value;

            var settings = new ElasticsearchClientSettings(new Uri(config.Url))
                .Authentication(new BasicAuthentication(config.Username, config.Password))
                .EnableDebugMode(); //remove this line in production

            return new ElasticsearchClient(settings);
        });

        return services;
    }
}
