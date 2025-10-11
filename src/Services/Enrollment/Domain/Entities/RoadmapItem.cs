using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemy.BuildingBlocks.Domain;

namespace Codemy.Enrollment.Domain.Entities
{
    internal class RoadmapItem : BaseEntity
    {
        public long roadmapId { get; set; }
        public int order { get; set; }
        public int courseId { get; set; }
    }
}
