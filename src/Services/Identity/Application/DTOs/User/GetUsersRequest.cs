using System.ComponentModel.DataAnnotations;

namespace Codemy.Identity.Application.DTOs.User
{
    public class GetUsersRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        [RegularExpression("^(admin|moderator|instructor|student)$", ErrorMessage = "Role must be one of: admin, moderator, instructor, student.")]
        public string? Role { get; set; }
        [RegularExpression("^(active|inactive|pending)$", ErrorMessage = "Status must be one of: active, inactive, pending.")]
        public string? Status { get; set; }
        public bool? EmailVerified { get; set; }
        [RegularExpression("^(name|email|rating)$", ErrorMessage = "SortBy must be one of: name, email, rating.")]
        public string? SortBy { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
        public int Page { get; set; } = 1;
        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;
    }
}