namespace Codemy.Review.Application.DTOs
{
    public class GetUserDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public int Role { get; set; }
        public int Status { get; set; }
        public bool EmailVerified { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public int? TotalCourses { get; set; }
        public decimal? Rating { get; set; }
    }
}