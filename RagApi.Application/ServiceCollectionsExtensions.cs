using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;
using RagApi.service;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRagServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new OllamaApiClient(new Uri(configuration["Ollama:BaseUrl"]!)));
        services.AddScoped<EmbeddingService>();

        return services;
    }
}