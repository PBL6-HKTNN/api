using System.ComponentModel.DataAnnotations;

namespace Codemy.Courses.Application.DTOs
{
    public class ChangeCourseStatusRequest
    {
        [Required]
        public required Guid CourseId { get; set; }
        [Required]
        public required int Status { get; set; }
        public Guid? ModeratorId { get; set; }
    }
}
