using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using Codemy.BuildingBlocks.Domain;
using Codemy.Enrollment.Domain.Enums;

namespace Codemy.Enrollment.Domain.Entities
{
    internal class Enrollment : BaseEntity
    {
        public long studentId { get; set; }
        public long courseId { get; set; }
        public ProgressStatus progressStatus {get; set; }
        public long lessonId { get; set;}
        public EnrollmentStatus enrollmentStatus { get; set; }
        public DateTime enrollmentDate { get; set; }
        public DateTime? completionDate { get; set; }
        public string? certificateUrl { get; set; }
        public DateTime? certificateExpiryDate { get; set; }
    } 
}
