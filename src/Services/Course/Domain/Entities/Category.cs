using Codemy.BuildingBlocks.Domain;

namespace Codemy.Courses.Domain.Entities
{
    internal class Category : BaseEntity
    {
        public string name { get; set; }
        public string description { get; set; }
    }
}
