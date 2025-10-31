using Codemy.BuildingBlocks.Domain;
using Codemy.Courses.Domain.Enums;

namespace Codemy.Courses.Domain.Entities
{
    public class Enrollment : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public Progress Progress { get; set; }  
        public int LessonId { get; set; }
        public EnrollmentStatus Status { get; set; }  
        public DateTime EnrolledAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? CertificateUrl { get; set; }
        public DateTime? CertificateExpiryDate { get; set; }

        // Navigation property
        public Course Course { get; set; }
    }
}
