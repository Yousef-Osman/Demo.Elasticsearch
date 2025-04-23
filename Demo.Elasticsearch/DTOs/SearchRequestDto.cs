namespace Demo.Elasticsearch.DTOs;

public class SearchRequestDto
{
    public string Query { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortField { get; set; }
    public bool SortAsc { get; set; } = false;
    public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
}
