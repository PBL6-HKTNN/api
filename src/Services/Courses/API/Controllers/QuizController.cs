using Codemy.Courses.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Application.DTOs;

namespace Codemy.Courses.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly ILogger<QuizController> _logger;
        public QuizController(IQuizService quizService, ILogger<QuizController> logger)
        {
            _quizService = quizService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizRequest request)
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
                var result = await _quizService.CreateQuizAsync(request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to create Quiz.",
                        "Quiz creation failed due to business logic constraints."
                    );
                }
                return this.CreatedResponse(
                    result.Quiz,
                    $"/quiz/get/{result.Quiz.Id}"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Quiz.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during Quiz creation",
                    ex.Message
                );
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuizById(Guid id)
        {
            try
            {
                var quiz = await _quizService.GetQuizByIdAsync(id);
                if (quiz == null)
                {
                    return this.NotFoundResponse("Quiz not found.");
                }
                return this.OkResponse(quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Quiz.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred while retrieving Quiz",
                    ex.Message
                );
            }
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] SubmitQuizRequest request)
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
                var result = await _quizService.SubmitQuizAsync(request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to submit Quiz.",
                        "Quiz submission failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting Quiz.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during Quiz submission",
                    ex.Message
                );
            }
        }

        [HttpGet("results/{lessonId}")]
        public async Task<IActionResult> GetQuizResults(Guid lessonId)
        {
            try
            {
                var results = await _quizService.GetQuizResultsAsync(lessonId);
                return this.OkResponse(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Quiz results.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred while retrieving Quiz results",
                    ex.Message
                );
            }
        }

        [HttpPost("Attempts/{quizId}")]
        public async Task<IActionResult> GetQuizAttempts(Guid quizId)
        {
            try
            {
                var attempts = await _quizService.GetQuizAttemptsAsync(quizId);
                return this.OkResponse(attempts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Quiz attempts.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred while retrieving Quiz attempts",
                    ex.Message
                );
            }
        }

        [HttpPost("update/{quizId}")]
        public async Task<IActionResult> UpdateQuiz(Guid quizId, [FromBody] CreateQuizRequest request)
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
                var result = await _quizService.UpdateQuizAsync(quizId, request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to update Quiz.",
                        "Quiz update failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result.Quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Quiz.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during Quiz update",
                    ex.Message
                );
            }
        }

        [HttpDelete("{quizId}")]
        public async Task<IActionResult> DeleteQuiz(Guid quizId)
        {
            try
            {
                var result = await _quizService.DeleteQuizAsync(quizId);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to delete Quiz.",
                        "Quiz deletion failed due to business logic constraints."
                    );
                }
                return this.OkResponse(new { Message = "Quiz deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Quiz.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during Quiz deletion",
                    ex.Message
                );
            }
        } }
    }
