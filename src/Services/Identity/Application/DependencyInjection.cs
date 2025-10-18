using Codemy.Identity.Application.Interfaces;
using Codemy.Identity.Application.Services;
using Codemy.NotificationProto;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Identity.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        { 
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<EmailSender>();

            services.AddGrpcClient<NotificationService.NotificationServiceClient>(options =>
            { 
                options.Address = new Uri("https://localhost:7187");
            }); 

            return services;
        }
    }
}
