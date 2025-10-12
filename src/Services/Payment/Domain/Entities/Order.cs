using Codemy.Payment.Domain.Enums;

namespace Codemy.Payment.Domain.Entities
{
    internal class Order
    {
        public Guid userId { get; set; }
        public decimal totalAmount { get; set; }
        public OrderStatus orderStatus { get; set; }
        public Guid paymentId { get; set; }
    }
}
