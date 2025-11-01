using Codemy.BuildingBlocks.Domain;

namespace Codemy.Enrollment.Domain.Entities
{
    public class WishlistItem : BaseEntity
    {
        public Guid userId { get; set; } 
        public Guid courseId { get; set; } 
    }
}
