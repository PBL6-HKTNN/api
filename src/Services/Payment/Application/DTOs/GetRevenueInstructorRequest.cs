using System.ComponentModel.DataAnnotations;

namespace Codemy.Payment.Application.DTOs
{
    public class GetRevenueInstructorRequest
    {
        [Required]
        public required Guid InstructorId { get; set; }
        public Guid? CourseId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
