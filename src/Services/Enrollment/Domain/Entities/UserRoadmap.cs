using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemy.BuildingBlocks.Domain;

namespace Codemy.Enrollment.Domain.Entities
{
    internal class UserRoadmap : BaseEntity
    {
        public long userId { get; set; }
        public long roadmapId { get; set; }
        public DateTime enrolledAt { get; set; }
        public DateTime? completedAt { get; set; }
        public decimal progress { get; set; }
    }
}
