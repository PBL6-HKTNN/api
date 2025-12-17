namespace Codemy.Enrollment.Application.DTOs
{
    public class GetRoadmapDetailResponse
    {
        public RoadmapDetailDto Roadmap { get; set; } = null!;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
    public class RoadmapDetailDto
    {
        public Guid RoadmapId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        public bool IsOwner { get; set; }
        public bool IsJoined { get; set; }

        public decimal Progress { get; set; }

        public List<Guid> CourseIds { get; set; } = new();
    }
}
