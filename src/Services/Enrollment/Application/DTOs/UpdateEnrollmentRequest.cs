using Codemy.Enrollment.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Codemy.Enrollment.Application.DTOs
{
    public class UpdateEnrollmentRequest
    {
        [Required]
        public required Guid EnrollmentId { get; set; }
        public ProgressStatus? ProgressStatus { get; set; }
        public Guid? LessonId { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? CertificateUrl { get; set; }
        public DateTime? CertificateExpiryDate { get; set; }
    }
}
