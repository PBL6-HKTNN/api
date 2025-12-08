using Codemy.Review.Application.Interfaces;
using Codemy.Review.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Codemy.IdentityProto;
using Codemy.CoursesProto;

namespace Codemy.Review.Application 
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IReviewService, ReviewService>();

            services.AddGrpcClient<IdentityService.IdentityServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcClients:Identity"]);
            });

            services.AddGrpcClient<CoursesService.CoursesServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcClients:Courses"]);
            });

            return services;
        }
    }
}