namespace Codemy.Enrollment.Domain.Entities
{
    internal class WishlistItem
    {
        public Guid wishlistId { get; set; }
        public Guid courseId { get; set; }
        public DateTime addedAt { get; set; }
    }
}
