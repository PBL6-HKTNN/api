using Codemy.BuildingBlocks.Domain;

namespace Codemy.Enrollment.Domain.Entities
{
    public class RoadmapItem : BaseEntity
    {
        public Guid roadmapId { get; set; }
        public int order { get; set; }
        public Guid courseId { get; set; }
    }
}
