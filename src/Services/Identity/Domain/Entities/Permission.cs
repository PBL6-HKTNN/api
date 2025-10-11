using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemy.BuildingBlocks.Domain;

namespace Codemy.Identity.Domain.Entities
{
    internal class Permission : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
