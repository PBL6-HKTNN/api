using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Enrollment.Domain.Entities
{
    internal class WishlistItem
    {
        public long wishlistId { get; set; }
        public long courseId { get; set; }
        public DateTime addedAt { get; set; }
    }
}
