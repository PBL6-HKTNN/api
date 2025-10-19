using Codemy.BuildingBlocks.Core;
using Codemy.Identity.API.DTOs.User;
using Codemy.Identity.Application.Interfaces;
using Codemy.Identity.Domain.Entities;

namespace Codemy.Identity.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IFileStorageClient _fileStorageClient;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IRepository<User> userRepository, IFileStorageClient fileStorageClient, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _fileStorageClient = fileStorageClient;
            _unitOfWork = unitOfWork;
        }

        public async Task<User> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found");

            user.name = request.Name;
            user.bio = request.Bio;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return user;
        }

        public async Task<string> UploadAvatarAsync(Guid userId, Stream fileStream, string fileName)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found");

            var uploadResult = await _fileStorageClient.UploadImageAsync(fileStream, fileName, "image/jpeg");
            if (uploadResult == null || uploadResult.Data == null)
                throw new InvalidOperationException("Failed to upload avatar image.");
            user.profilePicture = uploadResult.Data.Url;

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return uploadResult.Data.Url;
        }
    }
}
