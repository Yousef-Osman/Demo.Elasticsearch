using Demo.Elasticsearch.Configuration;
using Demo.Elasticsearch.Extensions;
using Demo.Elasticsearch.HostedServices;
using Demo.Elasticsearch.Services;
using Demo.Elasticsearch.Services.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ElasticSettings>(builder.Configuration.GetSection("Elasticsearch"));

builder.Services.AddElasticsearch();
builder.AddSerilog();

builder.Services.AddHostedService<ElasticsearchIndexInitializer>();
builder.Services.AddScoped(typeof(IElasticsearchService<>), typeof(ElasticsearchService<>));
builder.Services.AddScoped<IProductService, ProductService>();

Log.Information("Starting web host");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
