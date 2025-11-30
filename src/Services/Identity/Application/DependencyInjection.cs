using Codemy.Identity.Application.Interfaces;
using Codemy.Identity.Application.Services;
using Codemy.NotificationProto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Codemy.Identity.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            // Application-level services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<EmailSender>();

            // gRPC client - Notification service
            services.AddGrpcClient<NotificationService.NotificationServiceClient>(options =>
            { 
                options.Address = new Uri(configuration["GrpcClients:Notification"]);
            }); 

            return services;
        }
    }
}
