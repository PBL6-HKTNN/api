using Microsoft.AspNetCore.Mvc;
using Codemy.Search.Application.DTOs;
using Codemy.Search.Application.Interfaces;
using Codemy.BuildingBlocks.Core;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace Codemy.Search.API.Controllers
{
    [ApiController]
    [Route("search/courses")]
    public class CourseSearchController : ControllerBase
    {
        private readonly ICourseSearchService _searchService;

        public CourseSearchController(ICourseSearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpPost]
        [Authorize]
        [SwaggerOperation("SearchCourses", Summary = "Search for courses", Description = "Search for courses using a query string with pagination support")]
        public async Task<IActionResult> Search([FromBody] CourseSearchRequest request)
        {
            var result = await _searchService.SearchAsync(request);
            return this.OkResponse(result);
        }

        [HttpPost("index/bulk")]
        [SwaggerOperation("IndexCoursesBulk", Summary = "Index courses in bulk", Description = "Index multiple courses in bulk")]
        public async Task<IActionResult> IndexBulk([FromBody] IEnumerable<CourseIndexDto> courses)
        {
            await _searchService.IndexBulkAsync(courses);
            return this.OkResponse<object>(null);
        }
    }
}
