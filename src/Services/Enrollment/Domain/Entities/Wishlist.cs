using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemy.BuildingBlocks.Domain;

namespace Codemy.Enrollment.Domain.Entities
{
    internal class Wishlist : BaseEntity
    {
        public long userId { get; set; }
    }
}
