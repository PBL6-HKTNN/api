using Codemy.BuildingBlocks.Core;
using Codemy.Identity.API.DTOs.User;
using Codemy.Identity.Application.DTOs.User;
using Codemy.Identity.Application.Interfaces;
using Codemy.Identity.Domain.Entities;
using Codemy.Identity.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Action = Codemy.Identity.Domain.Entities.Action;

namespace Codemy.Identity.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserPermissionGroup> _userPermissionGroupRepository;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IRepository<PermissionGroup> _permissionGroupRepository;
        private readonly IRepository<Action> _actionRepository;

        private readonly ILogger<UserService> _logger;
        private readonly IFileStorageClient _fileStorageClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(
            IRepository<User> userRepository,
            IRepository<UserPermissionGroup> userPermissionGroupRepository,
            IRepository<Permission> permissionRepository,
            IRepository<PermissionGroup> permissionGroupRepository,
            IRepository<Action> actionRepository,
            ILogger<UserService> logger,
            IFileStorageClient fileStorageClient,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _userPermissionGroupRepository = userPermissionGroupRepository;
            _permissionRepository = permissionRepository;
            _permissionGroupRepository = permissionGroupRepository;
            _actionRepository = actionRepository;
            _logger = logger;
            _fileStorageClient = fileStorageClient;
            _httpContextAccessor = httpContextAccessor;
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
                "name" => query.OrderBy(u => u.name),
                "email" => query.OrderBy(u => u.email),
                "rating" => query.OrderByDescending(u => u.rating),
                _ => query.OrderByDescending(u => u.CreatedAt)
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

        public async Task<UserResponse> EditInformationUser(EditInformationRequest request)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new UserResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);

            var userExists = await _userRepository.GetByIdAsync(UserId);
            if (userExists == null || userExists.IsDeleted)
            {
                return new UserResponse
                {
                    Success = false,
                    Message = "User does not exist."
                };
            }

            if (request.name == null && request.bio == null)
            {
                return new UserResponse
                {
                    Success = false,
                    Message = "No information provided to update."
                };
            }

            if (request.name != null)
            {
                userExists.name = request.name;
            }

            if (request.bio != null)
            {
                userExists.bio = request.bio;
            }

            _userRepository.Update(userExists);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return new UserResponse
                {
                    Success = true,
                    Message = "User information updated successfully.",
                    User = userExists
                };
            }
            else
            {
                return new UserResponse
                {
                    Success = false,
                    Message = "Failed to update user information."
                };
            }
        }

        public async Task<UserDTOResponse> GetUserInfoByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
            {
                return new UserDTOResponse
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            var userPermissions = await _userPermissionGroupRepository
                .FindAsync(p => p.UserId == user.Id);

            // 2. Get permissions from role
            var rolePermissions = await _userPermissionGroupRepository
                .FindAsync(p => p.RoleId == user.role);

            // 3. Merge + remove duplicates by PermissionId
            var allPermissionGroups = userPermissions
                .Concat(rolePermissions)
                .GroupBy(p => p.PermissionId)
                .Select(g => g.First())
                .ToList();
            var permissions = new List<Permission>();
            foreach (var upg in allPermissionGroups)
            {
                _logger.LogInformation("Fetching permission with ID: {PermissionId} for user with ID: {UserId}", upg.PermissionId, id);
                var permission = await _permissionRepository.GetByIdAsync(upg.PermissionId);
                if (permission != null && !permission.IsDeleted)
                {
                    permissions.Add(permission);
                }
            }
            return new UserDTOResponse
            {
                Success = true,
                User = user,
                Permissions = permissions
            };
        }

        public async Task<ListActionResponse> GetListActionByUserIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
            {
                return new ListActionResponse
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            // 1. Get permissions from user
            var userPermissions = await _userPermissionGroupRepository
                .FindAsync(p => p.UserId == user.Id);

            // 2. Get permissions from role
            var rolePermissions = await _userPermissionGroupRepository
                .FindAsync(p => p.RoleId == user.role);

            // 3. Merge + remove duplicates by PermissionId
            var allPermissionGroups = userPermissions
                .Concat(rolePermissions)
                .GroupBy(p => p.PermissionId)
                .Select(g => g.First())
                .ToList();

            List<Guid> permissionIds = allPermissionGroups.Where(g => !g.IsDeleted).Select(g => g.PermissionId).ToList();

            List<Action> actions = new List<Action>();

            foreach (var permission in permissionIds)
            {
                var permissions = await _permissionGroupRepository.FindAsync(p => p.permissionId == permission);
                foreach (var permissionItem in permissions)
                {
                    Guid actionId = permissionItem.actionId;
                    Action action = await _actionRepository.GetByIdAsync(actionId);
                    if (action != null)
                    {
                        actions.Add(action);
                    }
                }
            }

            return new ListActionResponse
            {
                Success = true,
                Actions = actions.DistinctBy(a => a.Id).ToList()
            };
        }
    }
}
