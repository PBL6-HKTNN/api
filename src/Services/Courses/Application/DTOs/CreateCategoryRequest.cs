using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Courses.Application.DTOs
{
    public class CreateCategoryRequest
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Description { get; set; }
    }
}
