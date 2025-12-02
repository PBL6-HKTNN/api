using System.ComponentModel.DataAnnotations;

namespace Codemy.Identity.Application.DTOs.Request
{
    public class UpdateRequestDTO
    {
        [Required]
        public required string Description { get; set; }
    }
}
