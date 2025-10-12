using Codemy.BuildingBlocks.Domain;

namespace Codemy.Identity.Domain.Entities
{
    internal class Permission : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
