using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Notification.Application.DTOs
{
    public class EmailContent
    {
        [EmailAddress]
        public required string From { get; set; }
        [EmailAddress]
        public required string To { get; set; }
        public required string Token { get; set; }

    }
}
