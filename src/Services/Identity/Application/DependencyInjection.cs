using Microsoft.Extensions.DependencyInjection;
using Codemy.Identity.Application.Interfaces;
using Codemy.Identity.Application.Services;
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
            // Business Logic Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            // Add other application services as needed
            // services.AddScoped<IUserService, UserService>();
            // services.AddScoped<IRoleService, RoleService>();

            return services;
        }
    }
}
