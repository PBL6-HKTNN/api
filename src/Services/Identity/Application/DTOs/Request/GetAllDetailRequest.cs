using Codemy.Identity.Domain.Enums;

namespace Codemy.Identity.Application.DTOs.Request
{
    public class GetAllDetailRequest
    {
        public Guid? RequestTypeId { get; set; }
        public RequestStatus? Status { get; set; }
        public string? sort { get; set; }
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
    }
}
