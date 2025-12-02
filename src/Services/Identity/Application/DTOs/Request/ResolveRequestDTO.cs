using Codemy.Identity.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Codemy.Identity.Application.DTOs.Request
{
    public class ResolveRequestDTO
    {
        [Required]
        public required RequestStatus Status { get; set; }
        [Required]
        public required Guid RequestId { get; set; }
        [Required]
        public required string Response { get; set; }
    }
}
