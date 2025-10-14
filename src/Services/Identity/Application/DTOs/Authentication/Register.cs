using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Identity.Application.DTOs.Authentication
{
    public class Register
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string name { get; set; }
        public string Password { get; set; }
    }
}
