using Codemy.Courses.Application.Interfaces;
using Codemy.Courses.Application.Services;
using Codemy.EnrollmentsProto;
using Codemy.IdentityProto;
using Codemy.NotificationProto;
using Codemy.SearchProto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codemy.Courses.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddHostedService<HideCourseBackgroundService>();

            services.AddGrpcClient<IdentityService.IdentityServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcClients:Identity"]);
            });

            services.AddGrpcClient<CourseIndexService.CourseIndexServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcClients:Search"]);
            });
            services.AddGrpcClient<EnrollmentService.EnrollmentServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcClients:Enrollment"]);
            });
            services.AddGrpcClient<NotificationService.NotificationServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcClients:Notification"]);
            });
            return services;
        }
    }
}
