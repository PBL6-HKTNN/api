using Codemy.BuildingBlocks.Domain;


namespace Codemy.Payment.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public Guid userId { get; set; }
        public Guid courseId { get; set; }
        public decimal price { get; set; }
    }
}
