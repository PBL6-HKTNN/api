using Codemy.Identity.Application.DTOs.Permission;
using Codemy.Identity.Domain.Entities;
using Action = Codemy.Identity.Domain.Entities.Action;

namespace Codemy.Identity.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<AssignPermissionResponse> AssignPermissionAsync(AssignPermissionRequest request);
        Task<PermissionResponse> CreatePermissionAsync(CreatePermissionRequest request);
        Task<Response> DeletePermissionByIdAsync(Guid permissionId);
        Task<Response> DeleteUserPermissionByIdAsync(Guid id);
        Task<PermissionResponse> GetPermissionByIdAsync(Guid permissionId);
        Task<ListPermissionResponse> GetPermissionsAsync();
        Task<ListAssignPermissionResponse> GetPermissionsByRoleAsync(int role);
        Task<ListAssignPermissionResponse> GetPermissionsByUserAsync(Guid userId);
        Task<ListUsersResponse> GetUsersByPermissionIdAsync(Guid permissionId);
        Task<PermissionResponse> UpdatePermissionByIdAsync(Guid permissionId, CreatePermissionRequest request);
    }


    public class Response
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class ListUsersResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<User>? users { get; set; }
    }

    public class ListAssignPermissionResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<UserPermissionGroup>? userPermissionGroups { get; set; }
    }

    public class AssignPermissionResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public UserPermissionGroup? userPermissionGroup { get; set; }
    }

    public class ListPermissionResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<PermissionDTO>? permissionDTO { get; set; }
    }

    public class PermissionResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public PermissionDTO? permissionDTO { get; set; } 
    }

    public class PermissionDTO
    {
        public Guid Id { get; set; }
        public string permissionName { get; set; }
        public List<Action> actions { get; set; }
    }
}
