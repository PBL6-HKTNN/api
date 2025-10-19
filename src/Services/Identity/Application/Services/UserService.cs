using Codemy.Identity.Application.Interfaces;
using Codemy.Identity.Domain.Entities;

namespace Codemy.Identity.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public UserService(IUserRepository userRepository, ICloudinaryService cloudinaryService)
        {
            _userRepository = userRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<User> UpdateProfileAsync(Guid userId, string name, string bio)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found");

            user.Name = name;
            user.Bio = bio;

            await _userRepository.UpdateAsync(user);
            return user;
        }

        public async Task<string> UploadAvatarAsync(Guid userId, Stream fileStream, string fileName)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found");

            var avatarUrl = await _cloudinaryService.UploadImageAsync(fileStream, fileName);
            user.ProfilePicture = avatarUrl;

            await _userRepository.UpdateAsync(user);
            return avatarUrl;
        }
    }
}
