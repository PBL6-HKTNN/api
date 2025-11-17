using Codemy.Search.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Grpc.Net.ClientFactory;
using Codemy.CoursesProto;

namespace Codemy.Search.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            return services;
        }
    }
}