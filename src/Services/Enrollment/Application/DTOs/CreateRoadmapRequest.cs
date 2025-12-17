using System.ComponentModel.DataAnnotations;

namespace Codemy.Enrollment.Application.DTOs
{
    public class CreateRoadmapRequest
    {
        [Required]
        [MinLength(3, ErrorMessage = "Title must be at least 3 characters")]
        public string Title { get; set; } = null!;

        [Required]
        [MinLength(10, ErrorMessage = "Description must be at least 10 characters")]
        public string Description { get; set; } = null!;

        [Required]
        [MinLength(1, ErrorMessage = "At least one course is required")]
        public List<Guid> CourseIds { get; set; } = new();
    }
}
