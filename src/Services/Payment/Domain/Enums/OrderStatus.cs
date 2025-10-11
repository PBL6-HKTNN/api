using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Payment.Domain.Enums
{
    internal enum OrderStatus
    {
        Pending,
        Completed,
        Failed,
        Cancelled
    }
     
}
