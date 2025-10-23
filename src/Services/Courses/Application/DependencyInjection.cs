using Codemy.Courses.Application.Interfaces;
using Codemy.Courses.Application.Services;
using Codemy.IdentityProto;
using Microsoft.Extensions.DependencyInjection;

namespace Codemy.Courses.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        { 
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddGrpcClient<IdentityService.IdentityServiceClient>(options =>
            {
                options.Address = new Uri("https://localhost:7046");
            });
            return services;
        }
    }
}
