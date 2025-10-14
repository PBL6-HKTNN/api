using Codemy.BuildingBlocks.Domain;
using Codemy.Identity.Domain.Enums;


namespace Codemy.Identity.Domain.Entities
{
    public class User : BaseEntity
    { 
        public string name { get; set; }
        public string email { get; set; }
        public string googleId { get; set; }
        public string? passwordHash { get; set; }
        public Role role { get; set; }
        public UserStatus status { get; set; }
        public string profilePicture { get; set; }
        public string? bio { get; set; }
        public bool emailVerified { get; set; }
        public string? resetPasswordToken { get; set; }
        public DateTime? resetPasswordTokenExpiry { get; set; }
        public int totalCourses { get; set; }
        public decimal? rating { get; set; }
    }
}
