using Codemy.BuildingBlocks.Core; 
using Codemy.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;  
namespace Codemy.Identity.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddDbContext<IdentityDbContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));

            // Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>(); 


            return services;
        }
    }
}
