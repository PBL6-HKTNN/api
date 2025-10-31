using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Domain.Entities;

namespace Codemy.Courses.Application.Interfaces
{
    public interface ICourseService
    {
        Task<CourseReponse> CreateCourseAsync(CreateCourseRequest request);
        Task<CourseReponse> GetCourseByIdAsync(Guid courseId);
        Task<IEnumerable<MyCourseDto>> GetMyCoursesAsync(Guid studentId, int page = 1, int pageSize = 10);
    }

    public class CourseReponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; } 
        public Course? Course { get; set; } 
    }
}
