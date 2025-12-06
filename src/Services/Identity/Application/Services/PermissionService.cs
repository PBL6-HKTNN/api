using Codemy.BuildingBlocks.Core;
using Codemy.Identity.Application.DTOs.Permission;
using Codemy.Identity.Application.Interfaces;
using Codemy.Identity.Domain.Entities;
using Codemy.Identity.Domain.Enums;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Action = Codemy.Identity.Domain.Entities.Action;

namespace Codemy.Identity.Application.Services
{
    internal class PermissionService : IPermissionService
    {
        private readonly ILogger<PermissionService> _logger;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserPermissionGroup> _userPermissionRepository;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly IRepository<PermissionGroup> _permissionGroupRepository;
        private readonly IRepository<Action> _actionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;


        public PermissionService(
            ILogger<PermissionService> logger, 
            IRepository<User> userRepository,
            IRepository<UserPermissionGroup> userPermissionRepository, 
            IRepository<Permission> permissionRepository, 
            IRepository<PermissionGroup> permissionGroupRepository,
            IRepository<Action> actionRepository,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userPermissionRepository = userPermissionRepository;
            _permissionRepository = permissionRepository;
            _permissionGroupRepository = permissionGroupRepository;
            _actionRepository = actionRepository;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<AssignPermissionResponse> AssignPermissionAsync(AssignPermissionRequest request)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new AssignPermissionResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);

            var permission = await _permissionRepository.GetByIdAsync(request.PermissionId);
            if (permission == null || permission.IsDeleted)
            {
                return new AssignPermissionResponse
                {
                    Success = false,
                    Message = "Permission not found."
                };
            }
            UserPermissionGroup userPermissionGroup;
            if (request.UserId.HasValue)
            {
                var checkExist = await _userPermissionRepository.FindAsync(up => up.UserId == request.UserId && up.PermissionId == request.PermissionId && !up.IsDeleted);
                if (checkExist.Any())
                {
                    return new AssignPermissionResponse
                    {
                        Success = false,
                        Message = "Permission already assigned to this user."
                    };
                }
                User userAssign = await _userRepository.GetByIdAsync(request.UserId.Value);
                if (userAssign == null || userAssign.IsDeleted)
                {
                    return new AssignPermissionResponse
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }
                userPermissionGroup = new UserPermissionGroup
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    PermissionId = request.PermissionId,
                    CreatedBy = UserId,
                };
                await _userPermissionRepository.AddAsync(userPermissionGroup);
                _logger.LogInformation($"Assigned permission {request.PermissionId} to user {request.UserId}");
            }
            else
            {
                var checkExist = await _userPermissionRepository.FindAsync(up => up.RoleId == (Role)request.RoleId && up.PermissionId == request.PermissionId && !up.IsDeleted);
                if (checkExist.Any())
                {
                    return new AssignPermissionResponse
                    {
                        Success = false,
                        Message = "Permission already assigned to this role."
                    };
                }
                userPermissionGroup = new UserPermissionGroup
                {
                    Id = Guid.NewGuid(),
                    RoleId = (Role)request.RoleId,
                    PermissionId = request.PermissionId,
                    CreatedBy = UserId,
                };
                await _userPermissionRepository.AddAsync(userPermissionGroup);
                _logger.LogInformation($"Assigned permission {request.PermissionId} to role {request.RoleId}");
            }
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError($"Failed to assign permission {request.PermissionId}");
                return new AssignPermissionResponse
                {
                    Success = false,
                    Message = "Failed to assign permission."
                };
            }
            _logger.LogInformation($"Permission {request.PermissionId} assigned successfully.");
            return new AssignPermissionResponse
            {
                Success = true,
                Message = "Permission assigned successfully.",
                userPermissionGroup = userPermissionGroup
            };
        }

        public async Task<PermissionResponse> CreatePermissionAsync(CreatePermissionRequest request)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new PermissionResponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var UserId = Guid.Parse(userIdClaim);

            //check actionId 
            List<Action> actions = new List<Action>();
            foreach (Guid id in request.actionIds)
            {
                var action = await _actionRepository.GetByIdAsync(id);
                if (action == null)
                {
                    return new PermissionResponse
                    {
                        Success = false,
                        Message = $"Action with ID {id} does not exist."
                    };
                }
                if (action.IsDeleted)
                {
                    return new PermissionResponse
                    {
                        Success = false,
                        Message = $"Action with ID {id} is deleted."
                    };
                }
                actions.Add(action);
            }
            Guid permissionId = Guid.NewGuid();
            var permission = new Permission
            {
                Id = permissionId,
                permissionName = request.name,
            };
            await _permissionRepository.AddAsync(permission);

            foreach (Guid id in request.actionIds)
            {
                var permissionGroup = new PermissionGroup
                {
                    Id = Guid.NewGuid(),
                    permissionId = permissionId,
                    actionId = id,
                    CreatedBy = UserId,
                };
                await _permissionGroupRepository.AddAsync(permissionGroup);
            }
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new PermissionResponse
                {
                    Success = false,
                    Message = "Failed to create permission."
                };
            }
            return new PermissionResponse
            {
                Success = true,
                Message = "Permission created successfully.",
                permissionDTO = new PermissionDTO
                {
                    Id = permission.Id,
                    permissionName = permission.permissionName,
                    actions = actions,
                }
            };
        }

        public async Task<Response> DeletePermissionByIdAsync(Guid permissionId)
        {
            var permission = await _permissionRepository.GetByIdAsync(permissionId);
            if (permission == null || permission.IsDeleted)
            {
                return new Response
                {
                    Success = false,
                    Message = "Permission not found or already deleted."
                };
            }
            permission.IsDeleted = true;
            _permissionRepository.Update(permission);
            var permissionGroups = await _permissionGroupRepository.FindAsync(pg => pg.permissionId == permissionId && !pg.IsDeleted);
            foreach (var pg in permissionGroups)
            {
                pg.IsDeleted = true;
                _permissionGroupRepository.Update(pg);
            }
            var userPermissions = await _userPermissionRepository.FindAsync(up => up.PermissionId == permissionId && !up.IsDeleted);
            foreach (var up in userPermissions)
            {
                up.IsDeleted = true;
                _userPermissionRepository.Update(up);
            }
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new Response
                {
                    Success = false,
                    Message = "Failed to delete permission."
                };
            }
            return new Response
            {
                Success = true,
                Message = "Permission deleted successfully.",
            };
        }

        public async Task<Response> DeleteUserPermissionByIdAsync(Guid id)
        {
            var userPermission = await _userPermissionRepository.GetByIdAsync(id);
            if (userPermission == null || userPermission.IsDeleted)
            {
                return new Response
                {
                    Success = false,
                    Message = "User permission not found or already deleted."
                };
            }
            userPermission.IsDeleted = true;
            _userPermissionRepository.Update(userPermission);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new Response
                {
                    Success = false,
                    Message = "Failed to delete user permission."
                };
            }
            return new Response
            {
                Success = true,
                Message = "User permission deleted successfully.",
            };
        }

        public async Task<PermissionResponse> GetPermissionByIdAsync(Guid permissionId)
        {
            var permission = await _permissionRepository.GetByIdAsync(permissionId);
            if (permission == null || permission.IsDeleted)
            {
                return new PermissionResponse
                {
                    Success = false,
                    Message = "Permission not found."
                };
            }
            var permissionGroups = await _permissionGroupRepository.FindAsync(pg => pg.permissionId == permissionId && !pg.IsDeleted);
            List<Action> actions = new List<Action>();
            foreach (var pg in permissionGroups)
            {
                var action = await _actionRepository.GetByIdAsync(pg.actionId);
                if (action != null && !action.IsDeleted)
                {
                    actions.Add(action);
                }
            }
            return new PermissionResponse
            {
                Success = true,
                permissionDTO = new PermissionDTO
                {
                    Id = permission.Id,
                    permissionName = permission.permissionName,
                    actions = actions,
                }
            };
        }

        public async Task<ListPermissionResponse> GetPermissionsAsync()
        {
            var permissions = await _permissionRepository.GetAllAsync();
            List<PermissionDTO> permissionDTOs = new List<PermissionDTO>();
            foreach (var permission in permissions)
            {
                if (permission.IsDeleted) continue;
                var permissionGroups = await _permissionGroupRepository.FindAsync(pg => pg.permissionId == permission.Id && !pg.IsDeleted);
                List<Action> actions = new List<Action>();
                foreach (var pg in permissionGroups)
                {
                    var action = await _actionRepository.GetByIdAsync(pg.actionId);
                    if (action != null && !action.IsDeleted)
                    {
                        actions.Add(action);
                    }
                }
                permissionDTOs.Add(new PermissionDTO
                {
                    Id = permission.Id,
                    permissionName = permission.permissionName,
                    actions = actions,
                });
            }
            return new ListPermissionResponse
            {
                Success = true,
                permissionDTO = permissionDTOs
            };
        }

        public async Task<ListAssignPermissionResponse> GetPermissionsByRoleAsync(int role)
        {
            var userPermission = await _userPermissionRepository.FindAsync(u => (int)u.RoleId == role && !u.IsDeleted);
            if (userPermission.Count == 0)
            {
                return new ListAssignPermissionResponse
                {
                    Success = false,
                    Message = "No permission assign for this role."
                };
            }
            return new ListAssignPermissionResponse
            {
                Success = true,
                Message = "Get permission by user successfully",
                userPermissionGroups = userPermission.ToList()
            };
        }

        public async Task<ListAssignPermissionResponse> GetPermissionsByUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.IsDeleted)
            {
                return new ListAssignPermissionResponse
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            var userPermission = await _userPermissionRepository.FindAsync(u =>  (u.UserId ==  userId || u.RoleId == user.role) && !u.IsDeleted);
            return new ListAssignPermissionResponse
            {
                Success = true,
                Message = "Get permission by user successfully",
                userPermissionGroups = userPermission.ToList()
            };
        }

        public async Task<ListUsersResponse> GetUsersByPermissionIdAsync(Guid permissionId)
        {
            var userPermissions = await _userPermissionRepository.FindAsync(up => up.PermissionId == permissionId && !up.IsDeleted);
            if (userPermissions.Count == 0)
            {
                return new ListUsersResponse
                {
                    Success = false,
                    Message = "User not found for this permission."
                };
            }

            UserPermissionGroup userPermmission = userPermissions.First();
            List<User> users = new List<User>();

            //get user có user id, permission id trong userPermission
            if (userPermmission.UserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(userPermmission.UserId.Value);
                if (user != null)
                {
                    users.Add(user);
                }
            }

            //get user có role và permission id trong userPermission
            if (userPermmission.RoleId.HasValue)
            {
                var user = await _userRepository.FindAsync(u => u.role == userPermmission.RoleId.Value && !u.IsDeleted);
                if (user != null)
                {
                    users.AddRange(user);
                }
            }

            return new ListUsersResponse
            {
                Success = true,
                Message = "Get user of a permission successfully",
                users = users
            };
        }

        public async Task<PermissionResponse> UpdatePermissionByIdAsync(Guid permissionId, CreatePermissionRequest request)
        {
            var permission = await _permissionRepository.GetByIdAsync(permissionId);
            if (permission == null || permission.IsDeleted)
            {
                return new PermissionResponse
                {
                    Success = false,
                    Message = "Permission not found."
                };
            }

            List<Action> actions = new List<Action>();
            foreach (Guid id in request.actionIds)
            {
                var action = await _actionRepository.GetByIdAsync(id);
                if (action == null)
                {
                    return new PermissionResponse
                    {
                        Success = false,
                        Message = $"Action with ID {id} does not exist."
                    };
                }
                if (action.IsDeleted)
                {
                    return new PermissionResponse
                    {
                        Success = false,
                        Message = $"Action with ID {id} is deleted."
                    };
                }
                actions.Add(action);
            }

            permission.permissionName = request.name;
            _permissionRepository.Update(permission);
            var existingPermissionGroups = await _permissionGroupRepository.FindAsync(pg => pg.permissionId == permissionId && !pg.IsDeleted);
            foreach (var pg in existingPermissionGroups)
            {
                pg.IsDeleted = true;
                _permissionGroupRepository.Update(pg);
            }
            var user = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;
            var UserId = Guid.Parse(userIdClaim);
            foreach (Guid id in request.actionIds)
            {
                var permissionGroup = new PermissionGroup
                {
                    Id = Guid.NewGuid(),
                    permissionId = permissionId,
                    actionId = id,
                    CreatedBy = UserId,
                };
                await _permissionGroupRepository.AddAsync(permissionGroup);
            }
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new PermissionResponse
                {
                    Success = false,
                    Message = "Failed to update permission."
                };
            }

            return new PermissionResponse
            {
                Success = true,
                Message = "Permission updated successfully.",
                permissionDTO = new PermissionDTO
                {
                    Id = permission.Id,
                    permissionName = permission.permissionName,
                    actions = actions
                }
            };
        }
    }
}
