using Codemy.BuildingBlocks.Domain;
using Codemy.Payment.Domain.Enums;

namespace Codemy.Payment.Domain.Entities
{
    public class Payments : BaseEntity
    {  
        public DateTime paymentDate { get; set; }
        public MethodPayment method { get; set; }
        public Guid userId { get; set; }
        public decimal totalAmount { get; set; }
        public OrderStatus orderStatus { get; set; } 
    }
}
