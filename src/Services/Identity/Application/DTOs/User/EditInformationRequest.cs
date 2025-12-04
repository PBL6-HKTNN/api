using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Identity.Application.DTOs.User
{
    public class EditInformationRequest
    {
        public string? name { get; set; }
        public string? bio { get; set; }
    }
}
