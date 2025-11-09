using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Codemy.Courses.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(Summary = "Get course by ID", Description = "Retrieve a specific course by its ID")]
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

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateCourse([FromBody] ValidateCourseRequest request)
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
                var result = await _courseService.ValidateCourseAsync(request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Course validation failed."
                    );
                }
                return this.OkResponse("Course is valid.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating course.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during course validation",
                    ex.Message
                );
            }
        }

        [HttpGet("getModules/{courseId}")]
        [SwaggerOperation(Summary = "Get modules by course ID", Description = "Retrieve a list of modules for a specific course")]
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
                return this.OkResponse(modules.Modules);
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

        [HttpGet("getLessons/{courseId}")]
        public async Task<IActionResult> GetLessonByCourseIdAsync(Guid courseId)
        {
            if (courseId == Guid.Empty)
            {
                return BadRequest("Invalid course ID.");
            }
            try
            {
                var result = await _courseService.GetLessonByCourseIdAsync(courseId);
                if (result.Course == null || !result.Course.module.Any())
                {
                    return this.NotFoundResponse(
                        result.Message,
                        "No lessons found for the specified course."
                    );
                }
                return this.OkResponse(result.Course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lessons.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during lesson retrieval",
                    ex.Message
                );
            }
        }
        [HttpPost("create")]
        [SwaggerOperation(Summary = "Create a new course", Description = "Add a new course to the system")]
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
        [SwaggerOperation(Summary = "Update a course", Description = "Modify an existing course by its ID")]
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
        [SwaggerOperation(Summary = "Delete a course", Description = "Remove a course by its ID")]
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
        [SwaggerOperation(
            Summary = "Get all courses", 
            Description = "Retrieve a list of courses with optional filtering and pagination"
        )]
        public async Task<IActionResult> GetCourses([FromQuery] GetCoursesRequest request)
        {
            var result = await _courseService.GetCoursesAsync(
                request.CategoryId,
                request.Language,
                request.Level,
                request.SortBy,
                request.Page,
                request.PageSize
            );

            return this.OkResponse(result);
        }

    }
}
