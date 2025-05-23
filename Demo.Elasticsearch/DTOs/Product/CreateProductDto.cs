﻿namespace Demo.Elasticsearch.DTOs.Product;

public class CreateProductDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string Brand { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }
}
