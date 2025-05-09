using Demo.Elasticsearch.Common;
using Demo.Elasticsearch.DTOs;
using Demo.Elasticsearch.Models;

namespace Demo.Elasticsearch.Services.Interfaces;

public interface ISearchService<T> where T : class
{
    Task<Result<SearchResult<Product>>> SearchAsync(SearchRequestDto request);
}
