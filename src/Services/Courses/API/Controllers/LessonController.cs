using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Codemy.BuildingBlocks.Core;
using Microsoft.AspNetCore.Authorization;

namespace Codemy.Courses.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        private readonly ILogger<LessonController> _logger;
        public LessonController(ILessonService lessonService, ILogger<LessonController> logger)
        {
            _lessonService = lessonService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateLesson([FromBody] CreateLessonRequest request)
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
                var result = await _lessonService.CreateLessonAsync(request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to create Lesson.",
                        "Lesson creation failed due to business logic constraints."
                    );
                }
                return this.CreatedResponse(
                    result.Lesson,
                    $"/Lesson/get/{result.Lesson.Id}"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Lesson.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during Lesson creation",
                    ex.Message
                );
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetLessons()
        {
            try
            {
                var result = await _lessonService.GetLessons();
                if (result.Lessons.Count == 0)
                {
                    return this.NotFoundResponse("Lessons not found.",
                        "No lessons available in the system."
                    );
                }
                return this.OkResponse(result.Lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error get Lesson.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during Lesson retrieval",
                    ex.Message
                );
            }
        }

        [HttpGet("get/{lessonId}")]
        public async Task<IActionResult> GetLessonById(Guid lessonId)
        {
            try
            {
                var result = await _lessonService.GetLessonById(lessonId);
                if (!result.Success)
                {
                    return this.NotFoundResponse(
                        result.Message ?? "Lesson not found.",
                        "The specified lesson does not exist."
                    );
                }
                return this.OkResponse(result.Lesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lesson by ID.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during lesson retrieval",
                    ex.Message
                );
            }

        }
        [HttpPost("update/{lessonId}")]
        public async Task<IActionResult> UpdateLesson(Guid lessonId, [FromBody] CreateLessonRequest request)
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
                var result = await _lessonService.UpdateLessonAsync(lessonId, request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to update Lesson.",
                        "Lesson update failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result.Lesson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Lesson.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during Lesson update",
                    ex.Message
                );
            }
        }

        [HttpDelete("{lessonId}")]
        public async Task<IActionResult> DeleteLesson(Guid lessonId)
        {
            try
            {
                var result = await _lessonService.DeleteLessonAsync(lessonId);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to delete Lesson.",
                        "Lesson deletion failed due to business logic constraints."
                    );
                }
                return this.OkResponse("Lesson deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Lesson.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during Lesson deletion",
                    ex.Message
                );
            }
        } 
        } 
}
