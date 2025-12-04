using Codemy.BuildingBlocks.Domain;
using Codemy.Enrollment.Domain.Enums;

namespace Codemy.Enrollment.Domain.Entities
{
    public class Enrollments : BaseEntity
    {
        public Guid studentId { get; set; }
        public Guid courseId { get; set; }
        public ProgressStatus progressStatus {get; set; }
        public Guid? currentView { get; set; }
        public Guid? lessonId { get; set;}
        public EnrollmentStatus enrollmentStatus { get; set; }
        public DateTime enrollmentDate { get; set; }
        public DateTime expectedEndDate { get; set; }
        public DateTime? completionDate { get; set; }
        public string? certificateUrl { get; set; }
        public DateTime? certificateExpiryDate { get; set; }
    } 
}
