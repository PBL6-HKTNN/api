using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemy.BuildingBlocks.Domain;

namespace Codemy.Payment.Domain.Entities
{
    internal class Cart : BaseEntity
    {
        public long userId { get; set; }
    }
}
