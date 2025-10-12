using Codemy.BuildingBlocks.Domain;

namespace Codemy.Enrollment.Domain.Entities
{
    internal class Roadmap : BaseEntity
    {
        public Guid ownerId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    }
}
