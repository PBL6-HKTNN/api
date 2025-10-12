using Codemy.Payment.Domain.Enums;
using Codemy.BuildingBlocks.Domain;

namespace Codemy.Payment.Domain.Entities
{
    internal class Order : BaseEntity
    {
        public Guid userId { get; set; }
        public decimal totalAmount { get; set; }
        public OrderStatus orderStatus { get; set; }
        public Guid paymentId { get; set; }
    }
}
