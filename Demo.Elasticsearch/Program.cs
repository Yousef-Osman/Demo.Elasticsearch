using Demo.Elasticsearch.Configuration;
using Demo.Elasticsearch.HostedServices;
using Demo.Elasticsearch.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ElasticSettings>(builder.Configuration.GetSection("Elasticsearch"));

// Register ElasticsearchClient as Singleton
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IOptions<ElasticSettings>>().Value;

    var settings = new ElasticsearchClientSettings(new Uri(config.Url))
        .Authentication(new BasicAuthentication(config.Username, config.Password))
        .EnableDebugMode();

    return new ElasticsearchClient(settings);
});

builder.Services.AddScoped(typeof(IElasticService<>), typeof(ElasticService<>));
builder.Services.AddHostedService<ElasticsearchIndexInitializer>();


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
