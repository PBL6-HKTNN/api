using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemy.BuildingBlocks.Domain;


namespace Codemy.Payment.Domain.Entities
{
    internal class CartItem : BaseEntity
    {
        public long cartId { get; set; }
        public long courseId { get; set; }
        public decimal price { get; set; }
    }
}
