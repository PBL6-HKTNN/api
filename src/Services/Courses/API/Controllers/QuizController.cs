using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Core.Models;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [RequireAction("QUIZ_CREATE")]
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
        [RequireAction("QUIZ_READ")]
        public async Task<IActionResult> GetQuizById(Guid id)
        {
            try
            {
                var quiz = await _quizService.GetQuizByIdAsync(id);
                if (quiz == null)
                {
                    return this.NotFoundResponse("Quiz not found.");
                }
                return this.OkResponse(quiz.Quiz);
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

        [HttpGet("lessonId/{lessonId}")]
        [RequireAction("QUIZ_READ")]
        public async Task<IActionResult> GetQuizByLessonId(Guid lessonId)
        {
            try
            {
                var quiz = await _quizService.GetQuizByLessonIdAsync(lessonId);
                if (quiz == null)
                {
                    return this.NotFoundResponse("Quiz not found for the specified lesson.");
                }
                return this.OkResponse(quiz.Quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Quiz by Lesson ID.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred while retrieving Quiz by Lesson ID",
                    ex.Message
                );
            }
        }
        [HttpPost("submit")]
        [RequireAction("QUIZ_SUBMIT")]
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
                return this.OkResponse(result.QuizAttempts);
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
        [RequireAction("QUIZ_READ")]
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
        [HttpGet("list-results/{lessonId}")]
        [RequireAction("QUIZ_READ")]
        public async Task<IActionResult> GetListQuizResults(Guid lessonId)
        {
            try
            {
                var results = await _quizService.GetListQuizResultsAsync(lessonId);
                return this.OkResponse(results.QuizAttempts);
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

        [HttpGet("Attempts/{quizId}")]
        [RequireAction("QUIZ_READ")]
        public async Task<IActionResult> GetQuizAttempts(Guid quizId)
        {
            try
            {
                var attempts = await _quizService.GetQuizAttemptsAsync(quizId);
                if (!attempts.Success)
                {
                    return this.NotFoundResponse(attempts.Message ?? "Quiz attempts not found.",
                        attempts.Message ?? "No attempts found for the specified Quiz."
                    );
                }
                return this.OkResponse(attempts.QuizAttempt);
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
        [RequireAction("QUIZ_UPDATE")]
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
        [RequireAction("QUIZ_DELETE")]
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
