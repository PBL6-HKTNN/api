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
        public string email { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string password { get; set; }
    }
}
