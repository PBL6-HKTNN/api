using Codemy.Identity.API.DTOs.User;
using Codemy.Identity.Application.DTOs.User;
using Codemy.Identity.Domain.Entities;

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
    }
}