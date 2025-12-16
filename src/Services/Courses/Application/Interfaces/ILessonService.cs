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
        Task<LessonResponse> CheckLessonLocked(Guid lessonId);
        Task<LessonResponse> CreateLessonAsync(CreateLessonRequest request);
        Task<LessonResponse> DeleteLessonAsync(Guid lessonId);
        Task<LessonResponse> GetLessonById(Guid lessonId);
        Task<LessonListResponse> GetLessons();
        Task<VideoCheckpointReponse> GetVideoCheckpoint(Guid lessonId);
        Task<LessonResponse> UpdateLessonAsync(Guid lessonId, CreateLessonRequest request);
    }

    public class VideoCheckpointReponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public VideoCheckpointDTO? VideoCheckpoint { get; set; }
    }

    public class VideoCheckpointDTO
    {
        public Guid Id { get; set; }
        public Guid LessonId { get; set; }
        public string Time { get; set; }
        public string Question { get; set; }
        public List<string> Options { get; set; }
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
