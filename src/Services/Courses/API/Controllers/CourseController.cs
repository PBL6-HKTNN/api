using System.Security.Claims;
using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Codemy.BuildingBlocks.EventBus.Events;
using Codemy.BuildingBlocks.EventBus.RabbitMQ;
using Codemy.BuildingBlocks.Core.Models;

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

        [HttpPut("change-status")]
        [RequireAction("COURSE_UPDATE")]
        [SwaggerOperation(Summary = "Change course status", Description = "Update the status of a specific course")]
        public async Task<IActionResult> ChangeCourseStatus([FromBody] ChangeCourseStatusRequest request)
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
                var result = await _courseService.ChangeCourseStatusAsync(request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to change course status."
                    );
                }
                return this.OkResponse(result.Course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing course status.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during status change",
                    ex.Message
                );
            }
        }

        [HttpPut("mod-change-status")]
        [RequireAction("REQUEST_RESOLVE")]
        [SwaggerOperation(Summary = "Change course status by mod", Description = "Mod update the status of a specific course")]
        public async Task<IActionResult> ModChangeCourseStatus([FromBody] ChangeCourseStatusRequest request)
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
                var result = await _courseService.ModChangeCourseStatus(request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to change course status."
                    );
                }
                return this.OkResponse(result.Course);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing course status.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during status change",
                    ex.Message
                );
            }
        }

        [HttpPost("requested-ban-course/{courseId}")]
        [RequireAction("REQUEST_RESOLVE")]
        public async Task<IActionResult> RequestBanCourse(Guid courseId)
        {
            if (courseId == Guid.Empty)
            {
                return this.BadRequestResponse("Invalid course ID.");
            }
            try
            {
                var result = await _courseService.requestBanCourse(courseId);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to request ban course."
                    );
                }
                return this.OkResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting ban course.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during request ban course",
                    ex.Message
                );
            }
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

        [HttpPost("lessons-completed")]
        [RequireAction("COURSE_READ")]
        [SwaggerOperation(Summary = "Get completed lessons", Description = "Retrieve a list of completed lessons for a specific course and user")]
        public async Task<IActionResult> GetLessonsCompleted(GetLessonsCompletedRequest request)
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
                var course = await _courseService.GetLessonsCompletedAsync(request);
                if (!course.Success)
                {
                    return this.NotFoundResponse(
                        course.Message,
                        course.Message ?? "Failed to retrieve completed lessons."
                    );
                }
                return this.OkResponse(course.completedLessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving completed lessons.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during while retrieving completed lessons",
                    ex.Message
                );
            }
        }

        [HttpPost("validate")]
        [RequireAction("COURSE_READ")]
        [SwaggerOperation(Summary = "Validate course data", Description = "Validate if the lesson is the last lesson of a course")]
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


        [HttpPost("auto-check-before-submit")]
        [RequireAction("COURSE_CREATE")]
        [SwaggerOperation(Summary = "Auto check infor course before creating request", Description = "Validate if the course is the acceptable to create request public")]
        public async Task<IActionResult> AutoCheckCourse([FromBody] AutoCheckCourseRequest request)
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
                var result = await _courseService.AutoCheckCourseAsync(request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Course check failed."
                    );
                }
                return this.OkResponse("Course is valid.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking course.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during course check",
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
                return this.BadRequestResponse("Invalid course ID.");
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
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to retrieve lessons."
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
        [Authorize]
        [HttpPost("create")]
        [RequireAction("COURSE_CREATE")]
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
                // var publisher = new RabbitMqPublisher();
                // publisher.Publish(new CourseCreatedEvent
                // {
                //     Id = result.Course!.Id,
                //     InstructorId = result.Course!.instructorId,
                //     Title = result.Course!.title,
                //     Description = result.Course!.description,
                //     Thumbnail = result.Course!.thumbnail,
                //     Status = (int)result.Course!.status,
                //     Duration = result.Course!.duration,
                //     Price = result.Course!.price,
                //     Level = (int)result.Course!.level,
                //     NumberOfModules = result.Course!.numberOfModules,
                //     CategoryId = result.Course!.categoryId,
                //     Language = result.Course!.language,
                //     NumberOfReviews = result.Course!.numberOfReviews,
                //     AverageRating = result.Course!.averageRating
                // }, "course_created");

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
        [Authorize]
        [HttpPost("update/{courseId}")]
        [RequireAction("COURSE_UPDATE")]
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
        [Authorize]
        [HttpDelete("{courseId}")]
        [RequireAction("COURSE_DELETE")]
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

        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all courses", 
            Description = "Retrieve a list of courses with optional filtering and pagination"
        )]
        public async Task<IActionResult> GetCourses([FromQuery] GetCoursesRequest request)
        {
            var result = await _courseService.GetCoursesAsync(
                request.CategoryId,
                request.InstructorId,
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
