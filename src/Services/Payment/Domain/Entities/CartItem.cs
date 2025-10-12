using Codemy.BuildingBlocks.Domain;


namespace Codemy.Payment.Domain.Entities
{
    internal class CartItem : BaseEntity
    {
        public Guid cartId { get; set; }
        public Guid courseId { get; set; }
        public decimal price { get; set; }
    }
}
