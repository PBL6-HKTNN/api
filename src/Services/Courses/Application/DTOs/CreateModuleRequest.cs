using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Courses.Application.DTOs
{
    public class CreateModuleRequest
    {
        [Required]
        public Guid courseId { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        public int order { get; set; }
    }
}
