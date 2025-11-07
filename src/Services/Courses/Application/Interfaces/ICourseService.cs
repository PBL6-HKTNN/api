using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Domain.Entities;

namespace Codemy.Courses.Application.Interfaces
{
    public interface ICourseService
    {
        Task<CourseReponse> CreateCourseAsync(CreateCourseRequest request);
        Task<CourseReponse> DeleteCourseAsync(Guid courseId);
        Task<CourseReponse> GetCourseByIdAsync(Guid courseId);
        Task<ModuleListResponse> GetModuleByCourseIdAsync(Guid courseId);
        Task<CourseReponse> UpdateCourseAsync(Guid courseId, CreateCourseRequest request);
        Task<IEnumerable<Course>> GetCoursesAsync(
            Guid? categoryId = null,
            string? language = null,
            string? level = null,
            string? sortBy = null,
            int page = 1,
            int pageSize = 10);
    }

    public class CourseReponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; } 
        public Course? Course { get; set; } 
    }
}
