using Codemy.BuildingBlocks.Domain;
using Codemy.Identity.Domain.Enums;

namespace Codemy.Identity.Domain.Entities
{
    public class RequestType : BaseEntity
    {
        public RequestTypeEnum Type { get; set; }
        public string Description { get; set; }
    }
}
