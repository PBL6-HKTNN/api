using Codemy.BuildingBlocks.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Course.Domain.Entities
{
    internal class Category : BaseEntity
    {
        public string name { get; set; }
        public string description { get; set; }
    }
}
