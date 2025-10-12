using Codemy.BuildingBlocks.Domain;

namespace Codemy.Enrollment.Domain.Entities
{
    internal class Wishlist : BaseEntity
    {
        public Guid userId { get; set; }
    }
}
