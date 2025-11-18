using System.ComponentModel.DataAnnotations;

namespace Codemy.Identity.API.DTOs.User
{
    public class UpdateProfileRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        [RegularExpression(@"^[^<>'\""]*$", ErrorMessage = "Name contains invalid characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Bio must not exceed 500 characters")]
        [RegularExpression(@"^[^<>]*$", ErrorMessage = "Bio contains invalid characters")]
        public string Bio { get; set; }
    }
}
