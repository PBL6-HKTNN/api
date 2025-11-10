using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Core.Extensions;
using Codemy.Payment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codemy.Payment.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            LogExtensions.LoadEnvFile();
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var dbName = Environment.GetEnvironmentVariable("DB_PAYMENT");
            var dbSsl = string.Equals(Environment.GetEnvironmentVariable("DB_SSL"), "true", StringComparison.OrdinalIgnoreCase);

            var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Ssl Mode={(dbSsl ? "Require" : "Disable")};Trust Server Certificate=true;";

            services.AddDbContext<PaymentDbContext>(options =>
                options.UseNpgsql(connectionString));
            // Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            return services;
        }
    }
}
