using Codemy.BuildingBlocks.Domain;

namespace Codemy.Courses.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string name { get; set; }
        public string description { get; set; }
    }
}
