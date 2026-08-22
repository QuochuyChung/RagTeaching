using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qdrant.Client;
using RagApi.Infrastructure.Models;

namespace RagApi.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRagInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new QdrantClient(configuration["Qdrant:Host"]!, int.Parse(configuration["Qdrant:Port"]!)));

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.AddScoped<QdrantRepository>();
        services.AddScoped<DocumentRepository>();

        return services;
    }
}