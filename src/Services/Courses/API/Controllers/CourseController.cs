using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Codemy.Courses.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CourseController> _logger;
        public CourseController(ICourseService courseService, ILogger<CourseController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        [HttpGet("get/{courseId}")]
        public async Task<IActionResult> GetCourseById(Guid courseId)
        {
            if (courseId == Guid.Empty)
            {
                return BadRequest("Invalid course ID.");
            }
            try
            {
                var course = await _courseService.GetCourseByIdAsync(courseId);
                if (!course.Success)
                {
                    return this.NotFoundResponse(
                        "Course not found",
                        course.Message ?? "The specified course does not exist."
                    );
                }
                return this.OkResponse(course.Course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course."); 
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during register",
                    ex.Message
                );
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                    .Where(x => x.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return this.ValidationErrorResponse(validationErrors);
            }
            try
            {
                var result = await _courseService.CreateCourseAsync(request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to create course."
                    );
                }
                return this.CreatedResponse(result.Course!, $"/course/get/{result.Course!.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during course creation",
                    ex.Message
                );
            }
        }
    }
}
