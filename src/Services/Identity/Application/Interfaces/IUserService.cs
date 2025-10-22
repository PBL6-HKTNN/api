using Codemy.Identity.API.DTOs.User;
using Codemy.Identity.Domain.Entities;

namespace Codemy.Identity.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
        Task<string> UploadAvatarAsync(Guid userId, Stream fileStream, string fileName);
    }
}