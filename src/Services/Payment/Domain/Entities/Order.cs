using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemy.Payment.Domain.Enums;

namespace Codemy.Payment.Domain.Entities
{
    internal class Order
    {
        public long userId { get; set; }
        public decimal totalAmount { get; set; }
        public OrderStatus orderStatus { get; set; }
        public long paymentId { get; set; }

    }
}
