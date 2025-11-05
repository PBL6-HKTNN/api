namespace Codemy.Courses.Application.DTOs
{
    public class SubmitQuizRequest
    {
        public Guid QuizAttemptId { get; set; }
        public List<QuizAnswerDto> Answers { get; set; }
    }

    public class QuizAnswerDto
    {
        public Guid QuestionId { get; set; }
        public string? answerText { get; set; }
        public List<Guid>? SelectedAnswerIds { get; set; }
    }
}
