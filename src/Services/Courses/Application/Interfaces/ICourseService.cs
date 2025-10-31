using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Domain.Entities;
using Codemy.Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Courses.Application.Interfaces
{
    public interface ICourseService
    {
        Task<CourseReponse> CreateCourseAsync(CreateCourseRequest request);
        Task<CourseReponse> GetCourseByIdAsync(Guid courseId);
        Task<IEnumerable<CourseDto>> GetCoursesAsync(
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
