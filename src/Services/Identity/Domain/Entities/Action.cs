using Codemy.BuildingBlocks.Domain;

namespace Codemy.Identity.Domain.Entities
{
    public class Action : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        //example "create_user", "delete_user", "update_user"
    }
}
