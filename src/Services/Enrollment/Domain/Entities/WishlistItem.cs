using Codemy.BuildingBlocks.Domain;

namespace Codemy.Enrollment.Domain.Entities
{
    internal class WishlistItem : BaseEntity
    {
        public Guid wishlistId { get; set; }
        public Guid courseId { get; set; }
        public DateTime addedAt { get; set; }
    }
}
