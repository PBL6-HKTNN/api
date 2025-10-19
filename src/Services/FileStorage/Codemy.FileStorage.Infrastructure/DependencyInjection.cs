using Codemy.FileService.Infrastructure.Cloudinary;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DotNetEnv;

namespace Codemy.FileService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Load .env nếu chạy local
            Env.Load();

            // Lấy biến môi trường
            var cloudName = Environment.GetEnvironmentVariable("CLOUDINARY_NAME");
            var apiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
            var apiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");

            // Kiểm tra lỗi
            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                throw new Exception("Missing Cloudinary environment variables");
            }

            // Đăng ký CloudinarySettings
            var settings = new CloudinarySettings
            {
                CloudName = cloudName!,
                ApiKey = apiKey!,
                ApiSecret = apiSecret!,
                Folder = Environment.GetEnvironmentVariable("CLOUDINARY_FOLDER") ?? "codemy"
            };

            services.AddSingleton(settings);

            // Tạo Cloudinary client và inject
            var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
            var cloudinary = new Cloudinary(account);
            services.AddSingleton(cloudinary);

            // Đăng ký service
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IFileService, CloudinaryService>();

            return services;
        }
    }
}
