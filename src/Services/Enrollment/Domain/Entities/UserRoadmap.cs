using Codemy.BuildingBlocks.Domain;

namespace Codemy.Enrollment.Domain.Entities
{
    internal class UserRoadmap : BaseEntity
    {
        public Guid userId { get; set; }
        public Guid roadmapId { get; set; }
        public DateTime enrolledAt { get; set; }
        public DateTime? completedAt { get; set; }
        public decimal progress { get; set; }
    }
}
