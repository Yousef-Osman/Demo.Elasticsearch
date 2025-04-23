namespace Demo.Elasticsearch.DTOs;

public class SearchResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public long Total { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
}
