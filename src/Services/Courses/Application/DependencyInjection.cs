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
                options.Address = new Uri("https://localhost:7046");
            });

            services.AddGrpcClient<CourseIndexService.CourseIndexServiceClient>(options =>
            {
                options.Address = new Uri("https://localhost:7201");
            });
            return services;
        }
    }
}
