using Codemy.BuildingBlocks.Domain;

namespace Codemy.Enrollment.Domain.Entities
{
    internal class RoadmapItem : BaseEntity
    {
        public Guid roadmapId { get; set; }
        public int order { get; set; }
        public int courseId { get; set; }
    }
}
