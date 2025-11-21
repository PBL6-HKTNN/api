using Codemy.Courses.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Codemy.Courses.Application.DTOs
{
    public class CreateCourseRequest
    {
        [Required]
        public required Guid instructorId { get; set; }
        [Required]
        public required string title { get; set; }
        [Required]
        public required string description { get; set; }
        [Required]
        public required string thumbnail { get; set; }
        [Required]
        public required decimal price { get; set; }
        [Required]
        public required Level level { get; set; }
        [Required]
        public required Guid categoryId { get; set; }
        [Required]
        public required string language { get; set; } 
    }
}
