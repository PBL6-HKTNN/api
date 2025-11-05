using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Courses.Application.DTOs
{
    public class CreateQuizRequest
    {
        [Required]
        public Guid LessonId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int passingMarks { get; set; }
        [Required]
        public List<QuizQuestionDto> Questions { get; set; }
    }

    public class QuizQuestionDto
    {
        [Required]
        public Guid QuestionId { get; set; }
        [Required]
        public string QuestionText { get; set; }
        [Required]
        public int QuestionType { get; set; }
        [Required]
        public int Marks { get; set; }
        [Required]
        public List<AnswerDto> Answers { get; set; }
    }

    public class AnswerDto
    {
        [Required] 
        public Guid AnswerId { get; set; }
        [Required]
        public string AnswerText { get; set; }
        [Required]
        public bool IsCorrect { get; set; }
    }
}
