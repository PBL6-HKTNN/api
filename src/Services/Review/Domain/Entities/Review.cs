using Codemy.BuildingBlocks.Domain;

namespace Codemy.Review.Domain.Entities
{
    internal class Review : BaseEntity
    {
        public Guid courseId { get; set; }
        public Guid userId { get; set; }
        public int rating { get; set; }
        public string comment { get; set; }
    }
}
