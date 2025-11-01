using Codemy.CoursesProto;
using Codemy.Enrollment.Application.Interfaces;
using Codemy.Enrollment.Application.Services;
using Codemy.IdentityProto;
using Microsoft.Extensions.DependencyInjection;

namespace Codemy.Enrollment.Application
{
    public static class DependencyInjection
    {        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddGrpcClient<IdentityService.IdentityServiceClient>(options =>
            {
                options.Address = new Uri("https://localhost:7046");
            });
            services.AddGrpcClient<CoursesService.CoursesServiceClient>(options =>
            {
                options.Address = new Uri("https://localhost:7024");
            });
            return services;
        }
    }
}
