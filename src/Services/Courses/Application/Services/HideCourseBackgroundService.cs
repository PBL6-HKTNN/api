using Codemy.Courses.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Courses.Application.Services
{
    internal class HideCourseBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<HideCourseBackgroundService> _logger;

        public HideCourseBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<HideCourseBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var paymentService = scope.ServiceProvider.GetRequiredService<ICourseService>();
                    await paymentService.HideCoursesAutomatic();
                }

                _logger.LogInformation("Next check in 3 hours...");
                await Task.Delay(TimeSpan.FromHours(3), stoppingToken);
            }
        }
    }
}
