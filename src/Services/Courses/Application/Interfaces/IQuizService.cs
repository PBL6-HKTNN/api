using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Courses.Application.Interfaces
{
    public interface IQuizService
    {
        Task<QuizResponse> CreateQuizAsync(CreateQuizRequest request);
        Task<QuizResponse> DeleteQuizAsync(Guid quizId);
        Task<QuizAttemptDtoResponse> GetQuizAttemptsAsync(Guid quizId);
        Task<QuizDtoResponse> GetQuizByIdAsync(Guid id);
        Task<QuizDtoResponse> GetQuizByLessonIdAsync(Guid lessonId);
        Task<QuizResult> GetQuizResultsAsync(Guid lessonId);
        Task<QuizResult> SubmitQuizAsync(SubmitQuizRequest request);
        Task<QuizResponse> UpdateQuizAsync(Guid quizId, CreateQuizRequest request);
    }

    public class QuizResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public QuizAttemptResult? QuizAttempts { get; set; }
    }

    public class QuizAttemptResult
    {
        public Guid? QuizId { get; set; }
        public int Score { get; set; }
        public bool Passed { get; set; }
        public List<UserAnswer>? UserAnswers { get; set; }
    }

    public class QuizResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Quiz? Quiz { get; set; }
    } 
    public class QuizAttemptResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public QuizAttempt? QuizAttempt { get; set; }
    }

    public class QuizAttemptDtoResponse
    { 
        public bool Success { get; set; }
        public string? Message { get; set; }
        public QuizAttemptDto? QuizAttempt { get; set; }
    }

    public class QuizAttemptDto
    {
        public QuizAttempt QuizAttempt { get; set; }
        public QuizDto Quiz { get; set; }
    }

    public class QuizDtoResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public QuizDto? Quiz { get; set; }
    }
    public class QuizDto
    {
        public Guid Id { get; set; }
        public Guid LessonId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
        public List<QuizQuestionDto> Questions { get; set; }
    }
}
