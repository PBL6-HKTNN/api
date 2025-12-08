using System.ComponentModel.DataAnnotations;

namespace Codemy.Identity.Application.DTOs.Permission
{
    public class CreatePermissionRequest
    {
        [Required]
        public required string name { get; set; }
        [Required]
        public required List<Guid> actionIds { get; set; }
    }
}
