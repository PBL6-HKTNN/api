using Grpc.Core;
using Codemy.Search.Application.Interfaces;
using Codemy.SearchProto;

namespace Codemy.Search.API.Services
{
    public class CourseIndexGrpcService : CourseIndexService.CourseIndexServiceBase
    {
        private readonly ICourseSearchService _searchService;

        public CourseIndexGrpcService(ICourseSearchService searchService)
        {
            _searchService = searchService;
        }

        public override async Task<CourseIndexResponse> IndexCourse(CourseIndexRequest request, ServerCallContext context)
        {
            var dto = new Codemy.Search.Application.DTOs.CourseIndexDto
            {
                Id = Guid.Parse(request.Id),
                InstructorId = Guid.Parse(request.InstructorId),
                Title = request.Title,
                Description = request.Description,
                Thumbnail = request.Thumbnail,
                Status = request.Status,
                Duration = TimeSpan.Parse(request.Duration),
                Price = (decimal)request.Price,
                Level = request.Level,
                NumberOfModules = request.NumberOfModules,
                CategoryId = Guid.Parse(request.CategoryId),
                Language = request.Language,
                NumberOfReviews = request.NumberOfReviews,
                AverageRating = (decimal)request.AverageRating
            };

            await _searchService.IndexBulkAsync(new[] { dto });

            return new CourseIndexResponse { Success = true, Message = "Indexed" };
        }
    }
}
