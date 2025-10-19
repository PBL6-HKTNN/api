using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Codemy.BuildingBlocks.Core.Extensions;
using Codemy.FileStorage.Infrastructure.Configurations;
using Codemy.FileStorage.Application.Interfaces;
using Codemy.FileStorage.Infrastructure.Cloudinary;

namespace Codemy.FileStorage.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            LogExtensions.LoadEnvFile();

            var cloudName = Environment.GetEnvironmentVariable("CLOUDINARY_NAME");
            var apiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
            var apiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");

            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                throw new Exception("Missing Cloudinary environment variables");
            }

            // CloudinarySettings
            var settings = new CloudinarySettings
            {
                CloudName = cloudName!,
                ApiKey = apiKey!,
                ApiSecret = apiSecret!,
                // Folder = Environment.GetEnvironmentVariable("CLOUDINARY_FOLDER") ?? "codemy"
            };

            services.AddSingleton(settings);

            // Cloudinary instance
            var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
            var cloudinary = new CloudinaryDotNet.Cloudinary(account);
            services.AddSingleton(cloudinary);

            // Register services
            services.AddScoped<IFileService, CloudinaryService>();

            return services;
        }
    }
}
