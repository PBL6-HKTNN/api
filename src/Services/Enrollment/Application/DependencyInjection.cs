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
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddGrpcClient<IdentityService.IdentityServiceClient>(options =>
            {
                options.Address = new Uri("http://identity-service:5198");
            });
            services.AddGrpcClient<CoursesService.CoursesServiceClient>(options =>
            {
                options.Address = new Uri("http://courses-service:5078");
            });
            return services;
        }
    }
}
