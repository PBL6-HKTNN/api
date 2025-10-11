using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemy.BuildingBlocks.Domain;
using Codemy.Payment.Domain.Enums;

namespace Codemy.Payment.Domain.Entities
{
    internal class Payment : BaseEntity
    {
        public long orderId { get; set; }
        public decimal amount { get; set; }
        public DateTime paymentDate { get; set; }
        public MethodPayment method { get; set; }

    }
}
