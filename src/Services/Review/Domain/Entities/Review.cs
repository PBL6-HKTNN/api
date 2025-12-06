using Codemy.BuildingBlocks.Domain;

namespace Codemy.Review.Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid courseId { get; set; }
        public Guid userId { get; set; }
        public int rating { get; set; }
        public string comment { get; set; }
        public string? reply { get; set; }
        public Guid? repliedBy { get; set; }   // instructor who replied
        public DateTime? repliedAt { get; set; }
    }
}
