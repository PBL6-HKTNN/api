using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Core.Extensions;
using Codemy.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Codemy.Identity.Infrastructure.Clients;
using Codemy.Identity.Application.Interfaces;

namespace Codemy.Identity.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Load environment variables
            LogExtensions.LoadEnvFile();

            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var dbName = Environment.GetEnvironmentVariable("DB_IDENTITY");
            var dbSsl = string.Equals(Environment.GetEnvironmentVariable("DB_SSL"), "true", StringComparison.OrdinalIgnoreCase);

            var connectionString =
                $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Ssl Mode={(dbSsl ? "Require" : "Disable")};Trust Server Certificate=true;";

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Repository + UnitOfWork
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Http Clients
            services.AddHttpClient<IFileStorageClient, FileStorageClient>(client =>
            {
                // Configure the base address and other settings
                client.BaseAddress = new Uri(configuration["Services:FileStorage:BaseUrl"]
                    ?? "http://localhost:5164");
                client.Timeout = TimeSpan.FromMinutes(10);
            });

            return services;
        }
    }
}
