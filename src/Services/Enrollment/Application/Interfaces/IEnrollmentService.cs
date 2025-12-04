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
        Task<EnrollmentResponse> GetCourseAsync(Guid courseId);
        Task<CheckEnrollmentsResponse> CheckEnrollmentsAsync(CheckEnrollmentsRequest request);
        Task<EnrollmentResponse> GetCourseWithGrpc(Guid courseId, Guid userId);
        Task<CoursesResponse> GetMyCoursesAsync(Guid userId, GetMyCourseRequest request);
        Task<EnrollmentResponse> UpdateProgressAsync(UpdateProgressRequest request);
        Task<EnrollmentResponse> UpdateCurrentView(UpdateCurrentViewRequest request);
        Task<LessonCompletedResponse> GetLessonsCompletedByEnrollmentIdAsync(Guid enrollmentId);
        Task<LastDateResponse> CheckLastDateCourseAsync(Guid courseId);
    }

    public class LastDateResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public DateTime? LastDate { get; set; }
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

    public class LessonCompletedResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<Guid>? CompletedLessonIds { get; set; }
    }
}
