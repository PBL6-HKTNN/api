using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Core.Models;
using Codemy.Identity.API.DTOs.User;
using Codemy.Identity.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}
