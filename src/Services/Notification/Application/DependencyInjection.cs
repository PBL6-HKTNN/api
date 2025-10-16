
using Codemy.Notification.Application.Interfaces;
using Codemy.Notification.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Codemy.Notification.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Business Logic Services
            services.AddScoped<IEmailService, EmailService>(); 

            return services;
        }
    }
}
