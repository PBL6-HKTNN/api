using Codemy.BuildingBlocks.EventBus.Events;
using Codemy.BuildingBlocks.EventBus.RabbitMQ;
using Codemy.CoursesProto;
using Codemy.Search.Application.Interfaces;

public class CourseEventSubscriber : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly CoursesService.CoursesServiceClient _coursesGrpcClient;
    private RabbitMqSubscriber _subscriber;

    public CourseEventSubscriber(
        IServiceProvider serviceProvider,
        CoursesService.CoursesServiceClient coursesGrpcClient)
    {
        _serviceProvider = serviceProvider;
        _coursesGrpcClient = coursesGrpcClient;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            // Get scoped service within scope
            var searchService = scope.ServiceProvider.GetRequiredService<ICourseSearchService>();

            // Get all old courses via gRPC
            var allCoursesReply = await _coursesGrpcClient.GetAllCoursesForIndexingAsync(new Google.Protobuf.WellKnownTypes.Empty());

            var dtos = allCoursesReply.Courses.Select(c => new Codemy.Search.Application.DTOs.CourseIndexDto
            {
                Id = Guid.Parse(c.Id),
                InstructorId = Guid.Parse(c.InstructorId),
                Title = c.Title,
                Description = c.Description,
                Thumbnail = c.Thumbnail,
                Status = c.Status,
                Duration = TimeSpan.FromTicks(c.DurationTicks),
                Price = decimal.Parse(c.Price),
                Level = c.Level,
                NumberOfModules = c.NumberOfModules,
                CategoryId = string.IsNullOrEmpty(c.CategoryId) ? Guid.Empty : Guid.Parse(c.CategoryId),
                Language = c.Language,
                NumberOfReviews = c.NumberOfReviews,
                AverageRating = string.IsNullOrEmpty(c.AverageRating) ? 0 : decimal.Parse(c.AverageRating)
            });

            await searchService.IndexBulkAsync(dtos);

            // Subscribe to new courses via RabbitMQ
            try
            {
                _subscriber = new RabbitMqSubscriber();
                _subscriber.Subscribe<CourseCreatedEvent>("course_created", async evt =>
                {
                    using var innerScope = _serviceProvider.CreateScope();
                    var innerSearchService = innerScope.ServiceProvider.GetRequiredService<ICourseSearchService>();

                    var dto = new Codemy.Search.Application.DTOs.CourseIndexDto
                    {
                        Id = evt.Id,
                        InstructorId = evt.InstructorId,
                        Title = evt.Title,
                        Description = evt.Description,
                        Thumbnail = evt.Thumbnail,
                        Status = 1,
                        Duration = TimeSpan.Zero,
                        Price = evt.Price,
                        Level = 0,
                        NumberOfModules = 0,
                        CategoryId = Guid.Empty,
                        Language = "en",
                        NumberOfReviews = 0,
                        AverageRating = 0
                    };
                    await innerSearchService.IndexBulkAsync(new[] { dto });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting up RabbitMQ subscriber: {ex.Message}");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _subscriber = null;
        return Task.CompletedTask;
    }
}
