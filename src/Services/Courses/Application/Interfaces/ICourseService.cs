using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Domain.Entities;

namespace Codemy.Courses.Application.Interfaces
{
    public interface ICourseService
    {
        Task<CourseReponse> CreateCourseAsync(CreateCourseRequest request);
        Task<CourseReponse> DeleteCourseAsync(Guid courseId);
        Task<CourseReponse> GetCourseByIdAsync(Guid courseId);
        Task<ResourceDtoResponse> GetLessonByCourseIdAsync(Guid courseId);
        Task<ModuleListResponse> GetModuleByCourseIdAsync(Guid courseId);
        Task<CourseReponse> UpdateCourseAsync(Guid courseId, CreateCourseRequest request);
        Task<IEnumerable<GetCoursesResponse>> GetCoursesAsync(
            Guid? categoryId = null,
            string? language = null,
            string? level = null,
            string? sortBy = null,
            int page = 1,
            int pageSize = 10);
        Task<ValidateCourseResponse> ValidateCourseAsync(ValidateCourseRequest request);
    }

    public class ValidateCourseResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public bool? isLastLesson { get; set; }
    }

    public class Response
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class CourseReponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; } 
        public Course? Course { get; set; } 
    }

    public class ResourceDtoResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public CourseDto Course { get; set; }
    }

    public class CourseDto
    {
        public Course course { get; set; }
        public List<ModuleDto> module { get; set; }
    }

    public class ModuleDto
    {
        public Guid Id { get; set; }
        public string title { get; set; }
        public TimeSpan duration { get; set; }
        public int numberOfLessons { get; set; }
        public int order { get; set; }
        public List<Lesson> lessons { get; set; }
    }
}
