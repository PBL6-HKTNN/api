using Codemy.BuildingBlocks.Domain;

namespace Codemy.Payment.Domain.Entities
{
    internal class OrderItem : BaseEntity
    {
        public Guid orderId { get; set; }
        public Guid courseId { get; set; }
        public decimal price { get; set; }
    }
}
