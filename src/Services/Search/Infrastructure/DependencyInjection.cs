using Codemy.BuildingBlocks.Core.Extensions;
using Codemy.Search.Application.Interfaces;
using Codemy.Search.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;

namespace Codemy.Search.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            LogExtensions.LoadEnvFile();
            
            var elasticUri = Environment.GetEnvironmentVariable("ELASTIC_URI") ?? "http://localhost:9200";
            var indexName = Environment.GetEnvironmentVariable("ELASTIC_INDEX") ?? "courses";

            var settings = new ConnectionSettings(new Uri(elasticUri))
                .DefaultIndex(indexName);

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);
            services.AddScoped<ICourseSearchService, ElasticCourseSearchService>();

            return services;
        }
    }
}
