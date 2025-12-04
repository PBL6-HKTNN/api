using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Core.Models;
using Codemy.Identity.API.DTOs.User;
using Codemy.Identity.Application.DTOs.User;
using Codemy.Identity.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserResponse = Codemy.Identity.API.DTOs.User.UserResponse;

namespace Codemy.Identity.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [RequireAction("USER_UPDATE")]
        [HttpPut("{id}/profile")]
        public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateProfileRequest request)
        {
            var user = await _userService.UpdateProfileAsync(id, request);
            return this.OkResponse(new UserResponse
            {
                Id = user.Id,
                Name = user.name,
                Email = user.email,
                ProfilePicture = user.profilePicture,
                Bio = user.bio
            });
        }

        [Authorize]
        [HttpPost("{id}/avatar")]
        [RequireAction("USER_UPDATE")]
        public async Task<IActionResult> UploadAvatar(Guid id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file");

            using var stream = file.OpenReadStream();
            var avatarUrl = await _userService.UploadAvatarAsync(id, stream, file.FileName);
            return this.OkResponse(new { AvatarUrl = avatarUrl });
        }

        [Authorize]
        [HttpPost("edit")]
        [RequireAction("USER_UPDATE")]
        public async Task<IActionResult> EditInformationUser([FromBody] EditInformationRequest request)
        {
            var result = await _userService.EditInformationUser(request);
            if (!result.Success)
            {
                return this.BadRequestResponse(result.Message ?? "Failed to edit information", result.Message);
            }
            else return this.OkResponse(result.User);
        }

        [Authorize]
        [HttpGet("{id}")]
        [RequireAction("USER_READ")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserInfoByIdAsync(id);
            if (!user.Success)
            {
                return this.BadRequestResponse(user.Message ?? "User not found", user.Message);
            }
            return this.OkResponse(user);
        }

        [Authorize]
        [HttpGet("list-actions/{id}")]
        [RequireAction("USER_READ")]
        public async Task<IActionResult> GetListActionByUserId(Guid id)
        {
            var user = await _userService.GetListActionByUserIdAsync(id);
            if (!user.Success)
            {
                return this.BadRequestResponse(user.Message ?? "List action not found", user.Message);
            }
            return this.OkResponse(user.Actions);
        }

        [Authorize]
        [HttpGet]
        [RequireAction("USER_READ")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetUsersRequest request)
        {
            var result = await _userService.GetAllUsersAsync(
                request.Name,
                request.Email,
                request.Role,
                request.Status,
                request.EmailVerified,
                request.SortBy,
                request.Page,
                request.PageSize
            );

            return this.OkResponse(result);
        }
    }
}
