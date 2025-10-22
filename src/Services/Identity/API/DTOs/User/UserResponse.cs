namespace Codemy.Identity.API.DTOs.User
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
    }
}
