using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemy.BuildingBlocks.Domain;

namespace Codemy.Enrollment.Domain.Entities
{
    internal class Roadmap : BaseEntity
    {
        public long ownerId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    }
}
