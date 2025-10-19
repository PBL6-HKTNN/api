using Codemy.FileStorage.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Codemy.FileStorage.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<FileStorageAppService>();
            return services;
        }
    }
}