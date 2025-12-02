using System.ComponentModel.DataAnnotations;

namespace Codemy.Identity.Application.DTOs.Request
{
    public class CreateRequestDTO
    {
        [Required]
        public required Guid RequestTypeId { get; set; }
        [Required]
        public required string Description { get; set; }
        public Guid? courseId { get; set; }
    }
}
