using Codemy.BuildingBlocks.Domain;
using Codemy.Identity.Domain.Enums;

namespace Codemy.Identity.Domain.Entities
{
    //1 user can have many permission groups
    public class UserPermissionGroup : BaseEntity
    {
        public Guid? UserId { get; set; }
        public Role? RoleId { get; set; }
        public Guid PermissionId { get; set; }
    }

}
