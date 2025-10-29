using Codemy.Search.Application.DTOs;

namespace Codemy.Search.Application.Interfaces
{
    public interface ICourseSearchService
    {
        Task<PagedResult<CourseSearchResult>> SearchAsync(CourseSearchRequest request);
        Task IndexBulkAsync(IEnumerable<CourseIndexDto> courses);
    }
}
