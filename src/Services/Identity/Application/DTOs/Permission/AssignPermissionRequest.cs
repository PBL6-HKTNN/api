using System.ComponentModel.DataAnnotations;

namespace Codemy.Identity.Application.DTOs.Permission
{
    public class AssignPermissionRequest
    {
        public Guid? UserId { get; set; }
        public int? RoleId { get; set; }
        [Required]
        public required Guid PermissionId { get; set; }
    }
}
