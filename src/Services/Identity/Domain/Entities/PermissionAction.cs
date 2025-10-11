using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemy.BuildingBlocks.Domain;

namespace Codemy.Identity.Domain.Entities
{
    internal class PermissionAction : BaseEntity
    {
        public long permissionId { get; set; }
        public long actionId { get; set; }
        public bool enabled { get; set; }
    }
}
