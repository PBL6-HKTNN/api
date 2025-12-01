using Codemy.BuildingBlocks.Core;
using Codemy.Identity.API.DTOs.User;
using Codemy.Identity.Application.DTOs.User;
using Codemy.Identity.Application.Interfaces;
using Codemy.Identity.Domain.Entities;
using Codemy.Identity.Domain.Enums;
using Microsoft.EntityFrameworkCore;

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

        public async Task<GetUsersResponse> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found");
            var response = new GetUsersResponse
            {
                Id = user.Id,
                Name = user.name,
                Email = user.email,
                Role = (int)user.role,
                Status = (int)user.status,
                EmailVerified = user.emailVerified,
                ProfilePicture = user.profilePicture,
                Bio = user.bio,
                TotalCourses = user.totalCourses,
                Rating = user.rating
            };
            return response;
        }

        public async Task<IEnumerable<GetUsersResponse>> GetAllUsersAsync(
            string? name = null,
            string? email = null,
            string? role = null,
            string? status = null,
            bool? emailVerified = null,
            string? sortBy = null,
            int page = 1,
            int pageSize = 10
        )
        {
            var query = _userRepository.Query().Where(u => !u.IsDeleted);

            if (!string.IsNullOrEmpty(name))
                query = query.Where(u => u.name.ToLower().Contains(name.ToLower()));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(u => u.email.StartsWith(email));

            if (!string.IsNullOrEmpty(role))
            {
                if (Enum.TryParse<Role>(role, true, out var parsedRole))
                {
                    query = query.Where(u => u.role == parsedRole);
                }
                else
                {
                    throw new ArgumentException($"Invalid role value: {role}", nameof(role));
                }
            }
            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<UserStatus>(status, true, out var parsedStatus))
                {
                    query = query.Where(u => u.status == parsedStatus);
                }
                else
                {
                    throw new ArgumentException($"Invalid status value: {status}", nameof(status));
                }
            }

            if (emailVerified.HasValue)
                query = query.Where(u => u.emailVerified == emailVerified.Value);

            query = sortBy switch
            {
                "name"   => query.OrderBy(u => u.name),
                "email"  => query.OrderBy(u => u.email),
                "rating" => query.OrderByDescending(u => u.rating),
                _        => query.OrderByDescending(u => u.CreatedAt)
            };

            int skip = (page - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);


            var users = await query.ToListAsync();


            var result = users.Select(u => new GetUsersResponse
            {
                Id = u.Id,
                Name = u.name,
                Email = u.email,
                Role = (int)u.role,
                Status = (int)u.status,
                EmailVerified = u.emailVerified,
                ProfilePicture = u.profilePicture,
                Bio = u.bio,
                TotalCourses = u.totalCourses,
                Rating = u.rating
            });

            return result;
        }
    }
}
