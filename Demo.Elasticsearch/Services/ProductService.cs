using Demo.Elasticsearch.Common;
using Demo.Elasticsearch.Models;
using Demo.Elasticsearch.Services.Interfaces;
using Elastic.Clients.Elasticsearch;

namespace Demo.Elasticsearch.Services;

public class ProductService : ElasticService<Product>, IProductService
{
    public ProductService(ElasticsearchClient client) : base(client, Constants.IndexNames.Products) { }
}
