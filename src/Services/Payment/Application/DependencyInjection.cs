using Codemy.CoursesProto;
using Codemy.EnrollmentsProto;
using Codemy.IdentityProto;
using Codemy.Payment.Application.Interfaces;
using Codemy.Payment.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codemy.Payment.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        { 
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<PaymentGrpcEnrollmentService>();
            services.AddHostedService<PaymentStatusBackgroundService>();
            services.AddGrpcClient<IdentityService.IdentityServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcClients:Identity"]);
            });
            services.AddGrpcClient<CoursesService.CoursesServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcClients:Courses"]);
            });
            services.AddGrpcClient<EnrollmentService.EnrollmentServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcClients:Enrollment"]);
            });
            return services;
        }
    }
}
