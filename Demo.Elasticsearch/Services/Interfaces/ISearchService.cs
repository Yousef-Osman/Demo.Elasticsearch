using Demo.Elasticsearch.DTOs;

namespace Demo.Elasticsearch.Services.Interfaces;

public interface ISearchService<T> where T : class
{
    Task<SearchResult<T>> SearchAsync(SearchRequestDto request);
}
