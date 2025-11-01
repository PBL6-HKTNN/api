using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Courses.Application.Interfaces
{
    public interface ILessonService
    {
        Task<LessonResponse> CreateLessonAsync(CreateLessonRequest request);
        Task<LessonResponse> GetLessonById(Guid lessonId);
        Task<LessonListResponse> GetLessons();
    }

    public class LessonResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Lesson? Lesson { get; set; }
    }

    public class LessonListResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
