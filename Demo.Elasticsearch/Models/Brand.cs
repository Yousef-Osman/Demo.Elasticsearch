namespace Demo.Elasticsearch.Models;

public class Brand: BaseDocument
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string LogoUrl { get; set; }
}
