using Codemy.BuildingBlocks.Domain;

namespace Codemy.Payment.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid paymentId { get; set; }
        public Guid courseId { get; set; }
        public decimal price { get; set; }
    }
}
