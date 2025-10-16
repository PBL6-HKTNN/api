using Codemy.BuildingBlocks.Core; 
using Codemy.BuildingBlocks.Core.Extensions;
using Codemy.Identity.Infrastructure.Persistence;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codemy.Identity.Infrastructure
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
            var dbName = Environment.GetEnvironmentVariable("DB_IDENTITY");
              
            var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};Ssl Mode=Require;Trust Server Certificate=true;";
             
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseNpgsql(connectionString)); 
            // Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>(); 


            return services;
        }
    }
}
