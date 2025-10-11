using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemy.BuildingBlocks.Domain;
using Codemy.Course.Domain.Enum;
using Codemy.Course.Domain.Enums;

namespace Codemy.Course.Domain.Entities
{
    internal class Course : BaseEntity
    {
        public long instructorId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string thumbnail { get; set; }
        public Status status { get; set; }
        public TimeSpan duration { get; set; }
        public decimal price { get; set; }
        public Level level { get; set; }
        public long categoryId { get; set; }
        public string language { get; set; }
        public long numberOfReviews { get; set; }
        public decimal averageRating { get; set; }
    }
}
