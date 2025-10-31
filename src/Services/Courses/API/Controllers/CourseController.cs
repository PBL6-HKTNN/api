using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Codemy.Courses.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Codemy.Courses.API.Controllers
{
    [Authorize]
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

        [HttpGet("getModules/{courseId}")]
        public async Task<IActionResult> GetModuleByCourseIdAsync(Guid courseId)
        {
            if (courseId == Guid.Empty)
            {
                return BadRequest("Invalid course ID.");
            }
            try
            {
                var modules = await _courseService.GetModuleByCourseIdAsync(courseId);
                if (modules.Modules == null || !modules.Modules.Any())
                {
                    return this.NotFoundResponse(
                        modules.Message,
                        "No modules found for the specified course."
                    );
                }
                return this.OkResponse(modules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving modules."); 
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during module retrieval",
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

        [HttpPost("update/{courseId}")]
        public async Task<IActionResult> UpdateCourse(Guid courseId, [FromBody] CreateCourseRequest request)
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
                var result = await _courseService.UpdateCourseAsync(courseId, request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to update Course.",
                        "Course update failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result.Course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Course.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during course update",
                    ex.Message
                );
            }
        }

        [HttpDelete("{courseId}")]
        public async Task<IActionResult> DeleteCourse(Guid courseId)
        {
            try
            {
                var result = await _courseService.DeleteCourseAsync(courseId);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to delete Course.",
                        "Course deletion failed due to business logic constraints."
                    );
                }
                return this.OkResponse("Course deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Course.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during course deletion",
                    ex.Message
                );
            }
        }
        
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCourses(
            [FromQuery] Guid? categoryId,
            [FromQuery] string? language,
            [FromQuery] string? level,
            [FromQuery] string? sortBy,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _courseService.GetCoursesAsync(categoryId, language, level, sortBy, page, pageSize);
            return this.OkResponse(result);
        }
    }
}
