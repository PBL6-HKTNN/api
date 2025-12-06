using Codemy.BuildingBlocks.Domain;
using Codemy.Identity.Domain.Enums;

namespace Codemy.Identity.Domain.Entities
{
    public class Request : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid RequestTypeId { get; set; }
        public string Description { get; set; }
        public RequestStatus Status { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? ReviewId { get; set; }
        public string? Response { get; set; }
        //check updateBy to see who approved or rejected the request
    }
}
