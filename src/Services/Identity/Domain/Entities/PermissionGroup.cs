using Codemy.BuildingBlocks.Domain;

namespace Codemy.Identity.Domain.Entities
{
    public class PermissionGroup : BaseEntity
    {
        public Guid permissionId { get; set; }
        public Guid actionId { get; set; }
    }
}
