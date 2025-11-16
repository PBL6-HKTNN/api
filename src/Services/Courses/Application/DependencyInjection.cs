using Codemy.Courses.Application.Interfaces;
using Codemy.Courses.Application.Services;
using Codemy.IdentityProto;
using Codemy.SearchProto;
using Microsoft.Extensions.DependencyInjection;

namespace Codemy.Courses.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        { 
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddGrpcClient<IdentityService.IdentityServiceClient>(options =>
            {
                options.Address = new Uri("http://identity-service:5198");
            });

            services.AddGrpcClient<CourseIndexService.CourseIndexServiceClient>(options =>
            {
                options.Address = new Uri("http://search-service:5005");
            });
            return services;
        }
    }
}
