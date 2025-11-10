using Codemy.CoursesProto;
using Codemy.EnrollmentsProto;
using Codemy.IdentityProto;
using Codemy.Payment.Application.Interfaces;
using Codemy.Payment.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Codemy.Payment.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        { 
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<PaymentGrpcEnrollmentService>();
            services.AddHostedService<PaymentStatusBackgroundService>();
            services.AddGrpcClient<IdentityService.IdentityServiceClient>(options =>
            {
                options.Address = new Uri("https://localhost:7046");
            });
            services.AddGrpcClient<CoursesService.CoursesServiceClient>(options =>
            {
                options.Address = new Uri("https://localhost:7024");
            });
            services.AddGrpcClient<EnrollmentService.EnrollmentServiceClient>(options =>
            {
                options.Address = new Uri("https://localhost:7078");
            });
            return services;
        }
    }
}
