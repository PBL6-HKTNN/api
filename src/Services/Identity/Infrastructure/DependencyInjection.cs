using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Core.Extensions;
using Codemy.Identity.Infrastructure.Persistence;
using Codemy.Identity.Infrastructure.Repositories;
using Codemy.Identity.Application.Interfaces;
using Codemy.Identity.Infrastructure.Cloudinary;
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

            // Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();

            // Register Cloudinary
            services.AddScoped<ICloudinaryService, CloudinaryService>();

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddHttpClient<FileStorageClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["Services:FileStorage:BaseUrl"]
                    ?? "https://filestorage-service:5001");
            });

            return services;
        }
    }
}
