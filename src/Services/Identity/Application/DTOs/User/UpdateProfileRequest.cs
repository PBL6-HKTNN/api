namespace Codemy.Identity.API.DTOs.User
{
    public class UpdateProfileRequest
    {
        public string Name { get; set; } = default!;
        public string Bio { get; set; } = default!;
    }
}
