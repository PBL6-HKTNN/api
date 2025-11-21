using Codemy.Review.Application.Interfaces;
using Codemy.Review.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Codemy.Review.Application 
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Application-level services
            services.AddScoped<IReviewService, ReviewService>();

            return services;
        }
    }
}