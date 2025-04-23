using Demo.Elasticsearch.Models;

namespace Demo.Elasticsearch.Services.Interfaces;

public interface IProductService: IElasticsearchService<Product>, ISearchService<Product>
{
}
