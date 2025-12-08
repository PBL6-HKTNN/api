using System.ComponentModel.DataAnnotations;

namespace Codemy.Courses.Application.DTOs
{
    public class AutoCheckCourseRequest
    {
        [Required]
        public required Guid CourseId { get; set; }
    }
}
