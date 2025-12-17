namespace Codemy.Enrollment.Application.DTOs
{
    public class GetMyRoadmapsResponse
    {
        public List<RoadmapDto> Roadmaps { get; set; } = new List<RoadmapDto>();
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
    public class RoadmapDto
    {
        public Guid RoadmapId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Progress { get; set; }
    }
}
