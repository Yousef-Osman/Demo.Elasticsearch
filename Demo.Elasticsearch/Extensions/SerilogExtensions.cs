using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace Demo.Elasticsearch.Extensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        var applicationName = builder.Environment.ApplicationName;
        var indexFormat = $"{applicationName.ToLower().Replace(".", "-")}-logs-{DateTime.UtcNow:yyyy-MM}";
        var elasticUri = new Uri(builder.Configuration["Elasticsearch:Url"]);

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("ApplicationName", applicationName)
            .WriteTo.Console()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(elasticUri)
            {
                AutoRegisterTemplate = true,
                IndexFormat = indexFormat,
                NumberOfReplicas = 1,
                NumberOfShards = 2,
            })
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Services.AddSingleton(Log.Logger);
        builder.Host.UseSerilog();

        return builder;
    }
}
