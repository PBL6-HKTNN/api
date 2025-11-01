using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Domain.Entities;

namespace Codemy.Courses.Application.Interfaces
{
    public interface ICourseService
    {
        Task<CourseReponse> CreateCourseAsync(CreateCourseRequest request);
        Task<CourseReponse> GetCourseByIdAsync(Guid courseId);
        Task<ModuleListResponse> GetModuleByCourseIdAsync(Guid courseId);
    }

    public class CourseReponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; } 
        public Course? Course { get; set; } 
    }
}
