using Codemy.Identity.API.DTOs.User;
using Codemy.Identity.Application.DTOs.User;
using Codemy.Identity.Domain.Entities;
using Action = Codemy.Identity.Domain.Entities.Action;

namespace Codemy.Identity.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
        Task<string> UploadAvatarAsync(Guid userId, Stream fileStream, string fileName);
        Task<GetUsersResponse> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<GetUsersResponse>> GetAllUsersAsync(
            string? name = null,
            string? email = null,
            string? role = null,
            string? status = null,
            bool? emailVerified = null,
            string? sortBy = null,
            int page = 1,
            int pageSize = 10
        );
        Task<UserResponse> EditInformationUser(EditInformationRequest request);
        Task<UserDTOResponse> GetUserInfoByIdAsync(Guid id);
        Task<ListActionResponse> GetListActionByUserIdAsync(Guid id);
    }

    public class ListActionResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<Action>? Actions { get; set; }
    }

    public class UserDTOResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public User? User { get; set; }
        public List<Permission> Permissions { get; set; }
    }

    public class UserResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public User? User { get; set; }
    }
}