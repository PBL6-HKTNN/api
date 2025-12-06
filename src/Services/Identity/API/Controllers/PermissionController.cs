using Microsoft.AspNetCore.Mvc;
using Codemy.BuildingBlocks.Core;
using Codemy.Identity.Application.Interfaces;
using Codemy.BuildingBlocks.Core.Models;
using Codemy.Identity.Application.DTOs.Permission;


namespace Codemy.Identity.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(IPermissionService permissionService, ILogger<PermissionController> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }


        [HttpGet]
        [RequireAction("PERMISSION_READ")]
        public async Task<IActionResult> GetPermissions()
        {
            try
            {
                var result = await _permissionService.GetPermissionsAsync();
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to retrieve permissions.",
                        "Permission retrieval failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result.permissionDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permissions.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during permission retrieval",
                    ex.Message
                );
            }
        }

        [HttpGet("user-permission/{userId}")]
        [EndpointDescription("Get list user permission by userId")]
        [RequireAction("MY_PERMISSION_READ")]
        public async Task<IActionResult> GetPermissionsByUser(Guid userId)
        {
            try
            {
                var result = await _permissionService.GetPermissionsByUserAsync(userId);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to retrieve permissions.",
                        "Permission retrieval failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result.userPermissionGroups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permissions.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during permission retrieval",
                    ex.Message
                );
            }
        }

        [HttpGet("role-permission/{role}")]
        [EndpointDescription("Get list user permission by role")]
        [RequireAction("PERMISSION_READ")]
        public async Task<IActionResult> GetPermissionsByRole(int role)
        {
            try
            {
                var result = await _permissionService.GetPermissionsByRoleAsync(role);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to retrieve permissions.",
                        "Permission retrieval failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result.userPermissionGroups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permissions.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during permission retrieval",
                    ex.Message
                );
            }
        }

        [HttpGet("users/{permissionId}")]
        [EndpointDescription("Get list user by permissionId")]
        [RequireAction("PERMISSION_READ")]
        public async Task<IActionResult> GetUsersByPermissionId(Guid permissionId)
        {
            try
            {
                var result = await _permissionService.GetUsersByPermissionIdAsync(permissionId);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to retrieve users.",
                        "Users retrieval failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result.users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during user retrieval",
                    ex.Message
                );
            }
        }

        [HttpGet("get/{permissionId}")]
        [RequireAction("PERMISSION_READ")]
        public async Task<IActionResult> GetPermissionById(Guid permissionId)
        {
            try
            {
                var result = await _permissionService.GetPermissionByIdAsync(permissionId);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to retrieve permission.",
                        "Permission retrieval failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result.permissionDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permission by ID.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during permission retrieval by ID",
                    ex.Message
                );
            }
        }

        [HttpPost("create")]
        [RequireAction("PERMISSION_CREATE")]
        [EndpointDescription("Create new permission")]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                    .Where(x => x.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return this.ValidationErrorResponse(validationErrors);
            }
            try
            {
                var result = await _permissionService.CreatePermissionAsync(request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to create permission.",
                        "Permission creation failed due to business logic constraints."
                    );
                }
                return this.CreatedResponse(
                    result.permissionDTO,
                    $"/permission/get/{result.permissionDTO.Id}"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating permission.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during permission creation",
                    ex.Message
                );
            }
        }

        [HttpPost("assign")]
        [RequireAction("PERMISSION_CREATE")]
        [EndpointDescription("Assign user or role for a permission")]
        public async Task<IActionResult> AssignPermission([FromBody] AssignPermissionRequest request)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                    .Where(x => x.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return this.ValidationErrorResponse(validationErrors);
            }
            try
            {
                var result = await _permissionService.AssignPermissionAsync(request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to assign permission.",
                        "Permission assign failed due to business logic constraints."
                    );
                }
                return this.CreatedResponse(
                    result.userPermissionGroup,
                    ""
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assiging permission.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during permission assign",
                    ex.Message
                );
            }
        }

        [HttpDelete("delete/user-permission/{Id}")]
        [RequireAction("PERMISSION_DELETE")]
        public async Task<IActionResult> DeleteUserPermissionById(Guid Id)
        {
            try
            {
                var result = await _permissionService.DeleteUserPermissionByIdAsync(Id);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to delete user permission.",
                        "Permission deletion failed due to business logic constraints."
                    );
                }
                return this.OkResponse(new { Message = "User Permission deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user permission.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during user permission deletion",
                    ex.Message
                );
            }
        }

        [HttpDelete("delete/{permissionId}")]
        [RequireAction("PERMISSION_DELETE")]
        public async Task<IActionResult> DeletePermissionById(Guid permissionId)
        {
            try
            {
                var result = await _permissionService.DeletePermissionByIdAsync(permissionId);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to delete permission.",
                        "Permission deletion failed due to business logic constraints."
                    );
                }
                return this.OkResponse(new { Message = "Permission deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting permission.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during permission deletion",
                    ex.Message
                );
            }
        }

        [HttpPost("update/{permissionId}")]
        [RequireAction("PERMISSION_UPDATE")]
        public async Task<IActionResult> UpdatePermissionById(Guid permissionId, [FromBody] CreatePermissionRequest request)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                    .Where(x => x.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return this.ValidationErrorResponse(validationErrors);
            }
            try
            {
                var result = await _permissionService.UpdatePermissionByIdAsync(permissionId, request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to update permission.",
                        "Permission update failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result.permissionDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permission.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during permission update",
                    ex.Message
                );
            }
        }
    }
}


