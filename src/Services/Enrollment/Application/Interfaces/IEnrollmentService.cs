using Codemy.Enrollment.Application.DTOs;
using Codemy.Enrollment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Enrollment.Application.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentResponse> EnrollInCourseAsync(Guid courseId);
        Task<EnrollmentResponse> UpdateEnrollmentStatusAsync(UpdateEnrollmentRequest request);
    }

    public class EnrollmentResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Enrollments? Enrollment { get; set; }
    }
}
