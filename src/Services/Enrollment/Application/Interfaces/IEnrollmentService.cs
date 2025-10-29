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
        Task<EnrollmentResponse> EnrollInCourseAsyncWithoutGrpc(Guid courseId);
        Task<EnrollmentResponse> EnrollInCourseAsync(Guid courseId, Guid userId);
        Task<EnrollmentResponse> UpdateEnrollmentStatusAsync(UpdateEnrollmentRequest request);
        Task<Response> GetCourseAsync(Guid courseId);
        Task<CoursesResponse> GetMyCoursesAsync(Guid userId, int page, int pageSize);
    }

    public class EnrollmentResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Enrollments? Enrollment { get; set; }
    }

    public class CoursesResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<CourseDto>? Courses { get; set; }
    }
}
