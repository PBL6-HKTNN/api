using Codemy.Identity.API.DTOs.User;
using Codemy.Identity.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Codemy.Identity.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPut("{id}/profile")]
        public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateProfileRequest request)
        {
            var user = await _userService.UpdateProfileAsync(id, request.Name, request.Bio);
            return Ok(new UserResponse
            {
                Id = user.Id,
                Name = user.name,
                Email = user.email,
                ProfilePicture = user.profilePicture,
                Bio = user.bio
            });
        }

        [HttpPost("{id}/avatar")]
        public async Task<IActionResult> UploadAvatar(Guid id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file");

            using var stream = file.OpenReadStream();
            var avatarUrl = await _userService.UploadAvatarAsync(id, stream, file.FileName);
            return Ok(new { AvatarUrl = avatarUrl });
        }
    }
}
