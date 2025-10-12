using Codemy.BuildingBlocks.Domain;

namespace Codemy.Payment.Domain.Entities
{
    internal class Cart : BaseEntity
    {
        public Guid userId { get; set; }
    }
}
