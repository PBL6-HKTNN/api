using Codemy.BuildingBlocks.Domain;

namespace Codemy.Identity.Domain.Entities
{
    internal class Action : BaseEntity
    {
        public string actionName { get; set; }
        public string actionDescription { get; set; }
        public string actionCode { get; set; }
    }
}
