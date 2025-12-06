using Codemy.CoursesProto;
using Codemy.EnrollmentsProto;
using Codemy.Identity.Application.Interfaces;
using Codemy.Identity.Application.Services;
using Codemy.NotificationProto;
using Codemy.ReviewProto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codemy.Identity.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            // Application-level services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<EmailSender>();

            // gRPC client - Notification service
            services.AddGrpcClient<NotificationService.NotificationServiceClient>(options =>
            { 
                options.Address = new Uri(configuration["GrpcClients:Notification"]);
            });
            
            services.AddGrpcClient<CoursesService.CoursesServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcClients:Courses"]);
            });

            services.AddGrpcClient<EnrollmentService.EnrollmentServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcClients:Enrollments"]);
            });

            services.AddGrpcClient<ReviewService.ReviewServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcClients:Review"]);
            });

            return services;
        }
    }
}
