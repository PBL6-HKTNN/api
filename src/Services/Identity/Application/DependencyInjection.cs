using Codemy.Identity.Application.Interfaces;
using Codemy.Identity.Application.Services;
using Codemy.NotificationProto;
using Microsoft.Extensions.DependencyInjection;

namespace Codemy.Identity.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Application-level services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<EmailSender>();

            // gRPC client - Notification service
            services.AddGrpcClient<NotificationService.NotificationServiceClient>(options =>
            { 
                options.Address = new Uri("https://localhost:7187");
            }); 

            return services;
        }
    }
}
