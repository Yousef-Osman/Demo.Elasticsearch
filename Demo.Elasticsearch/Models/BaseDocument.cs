namespace Demo.Elasticsearch.Models;

public class BaseDocument
{
    public BaseDocument()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
