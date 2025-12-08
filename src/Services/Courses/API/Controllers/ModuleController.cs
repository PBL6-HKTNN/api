using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Core.Models;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codemy.Courses.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleService _moduleService;
        private readonly ILogger<ModuleController> _logger;
        public ModuleController(IModuleService moduleService, ILogger<ModuleController> logger)
        {
            _moduleService = moduleService;
            _logger = logger;
        }

        [HttpPost("create")]
        [RequireAction("MODULE_CREATE")]
        public async Task<IActionResult> CreateModule([FromBody] CreateModuleRequest request)
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
                var result = await _moduleService.CreateModuleAsync(request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to create Module.",
                        "Module creation failed due to business logic constraints."
                    );
                }
                return this.CreatedResponse(
                    result.Module,
                    $"/Module/get/{result.Module.Id}"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Module.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during Module creation",
                    ex.Message
                );
            }
        }
        [HttpGet]
        [RequireAction("MODULE_READ")]
        public async Task<IActionResult> GetModules()
        {
            try
            {
                var result = await _moduleService.GetModules();
                if (result.Modules.Count == 0)
                {
                    return this.NotFoundResponse("Modules not found.",
                        "No modules available in the system."
                    );
                }
                return this.OkResponse(result.Modules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error get Module.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during Module retrieval",
                    ex.Message
                );
            }
        }
        [HttpGet("{moduleId}")]
        [RequireAction("MODULE_READ")]
        public async Task<IActionResult> GetLessonByModuleId(Guid moduleId)
        {
            try
            {
                var result = await _moduleService.GetLessonByModuleId(moduleId);
                if (!result.Success)
                {
                    return this.BadRequest(result.Message ?? "Failed to get lessons by moduleId");
                }
                else if (result.Lessons.Count == 0)
                {
                    return this.NotFoundResponse(result.Message ?? "No lessons available in this module");
                }
                return this.OkResponse(result.Lessons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error get Lesson.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during Lesson retrieval",
                    ex.Message
                );
            }
        }
        [HttpGet("get/{moduleId}")]
        [RequireAction("MODULE_READ")]
        public async Task<IActionResult> GetModuleById(Guid moduleId)
        {
            try
            {
                var result = await _moduleService.GetModuleById(moduleId);
                if (!result.Success)
                {
                    return this.NotFoundResponse(
                        result.Message ?? "Module not found.",
                        "The specified module does not exist."
                    );
                }
                return this.OkResponse(result.Module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Module by ID.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during Module retrieval",
                    ex.Message
                );
            }
        }
        [HttpPost("update/{moduleId}")]
        [RequireAction("MODULE_UPDATE")]
        public async Task<IActionResult> UpdateModule(Guid moduleId, [FromBody] CreateModuleRequest request)
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
                var result = await _moduleService.UpdateModuleAsync(moduleId, request);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to update Module.",
                        "Module update failed due to business logic constraints."
                    );
                }
                return this.OkResponse(result.Module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Module.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during Module update",
                    ex.Message
                );
            }
        }

        [HttpDelete("{moduleId}")]
        [RequireAction("MODULE_DELETE")]
        public async Task<IActionResult> DeleteModule(Guid moduleId)
        {
            try
            {
                var result = await _moduleService.DeleteModuleAsync(moduleId);
                if (!result.Success)
                {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to delete Module.",
                        "Module deletion failed due to business logic constraints."
                    );
                }
                return this.OkResponse("Module deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Module.");
                return this.InternalServerErrorResponse(
                    "Internal server error occurred during Module deletion",
                    ex.Message
                );
            }
        }
    }
}
