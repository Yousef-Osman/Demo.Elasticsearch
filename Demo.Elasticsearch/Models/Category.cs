namespace Demo.Elasticsearch.Models;

public class Category: BaseDocument
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ParentCategoryId { get; set; }
}
