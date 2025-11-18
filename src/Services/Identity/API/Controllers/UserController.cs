using Codemy.BuildingBlocks.Core;
using Codemy.Identity.API.DTOs.User;
using Codemy.Identity.Application.Interfaces;
using Codemy.Identity.Domain.Entities;
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
        [HttpPut("{id}/profile")]
        public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateProfileRequest request)
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
