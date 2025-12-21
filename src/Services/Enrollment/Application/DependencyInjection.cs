using Codemy.CoursesProto;
using Codemy.Enrollment.Application.Interfaces;
using Codemy.Enrollment.Application.Services;
using Codemy.FilesProto;
using Codemy.IdentityProto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codemy.Enrollment.Application
{
    public static class DependencyInjection
    {        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IGoogleCalendarService, GoogleCalendarService>();
            services.AddScoped<IRoadmapService, RoadmapService>();

            services.AddScoped<ICertificateService, CertificateService>();
            services.AddGrpcClient<IdentityService.IdentityServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcClients:Identity"]);
            });
            services.AddGrpcClient<CoursesService.CoursesServiceClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcClients:Courses"]);
            });
            services.AddGrpcClient<FileUploader.FileUploaderClient>(options =>
            {
                options.Address = new Uri(configuration["GrpcClients:FileStorage"]);
            });
            return services;
        }
    }
}
