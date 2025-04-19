namespace Demo.Elasticsearch.Models;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; }
    public string Category { get; set; }
    public string Brand { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }
}
