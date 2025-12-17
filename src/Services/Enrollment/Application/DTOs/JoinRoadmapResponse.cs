using Codemy.Enrollment.Domain.Entities;

namespace Codemy.Enrollment.Application.DTOs
{
    public class JoinRoadmapResponse
    {
        public UserRoadmap UserRoadmap { get; set; } = null!;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
