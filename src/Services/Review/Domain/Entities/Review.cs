using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemy.BuildingBlocks.Domain;

namespace Codemy.Review.Domain.Entities
{
    internal class Review : BaseEntity
    {
        public long courseId { get; set; }
        public long userId { get; set; }
        public int rating { get; set; }
        public string comment { get; set; }
    }
}
