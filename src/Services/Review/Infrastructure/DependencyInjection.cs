using Codemy.BuildingBlocks.Core.Extensions;
using Codemy.Review.Application.Interfaces;
using Codemy.Review.Infrastructure.Persistence;
using Codemy.Review.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Codemy.Review.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Load environment variables
            LogExtensions.LoadEnvFile();

            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var dbName = Environment.GetEnvironmentVariable("DB_REVIEW");
            var dbSsl = string.Equals(Environment.GetEnvironmentVariable("DB_SSL"), "true", StringComparison.OrdinalIgnoreCase);

            var connectionString =
                $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Ssl Mode={(dbSsl ? "Require" : "Disable")};Trust Server Certificate=true;";

            services.AddDbContext<ReviewDbContext>(options =>
                options.UseNpgsql(connectionString));
            // Infrastructure-level services
            services.AddScoped<IReviewRepository, ReviewRepository>();

            return services;
        }
    }
}