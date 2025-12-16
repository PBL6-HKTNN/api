namespace Codemy.Courses.Application.DTOs
{
    public class CreateQuizInLessonRequest
    {
        public required Guid lessonId { get; set; }
        public required string time { get; set; }
        public required string question { get; set; }
        public required List<string> options { get; set; }
        public required string correctAnswer { get; set; }
    }
}
