using Codemy.BuildingBlocks.Domain;

namespace Codemy.Identity.Domain.Entities
{
    internal class PermissionAction : BaseEntity
    {
        public Guid permissionId { get; set; }
        public Guid actionId { get; set; }
        public bool enabled { get; set; }
    }
}
