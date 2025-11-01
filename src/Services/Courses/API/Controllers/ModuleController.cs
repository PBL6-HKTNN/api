using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Codemy.BuildingBlocks.Core;
using Microsoft.AspNetCore.Authorization;

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
                return StatusCode(500, "Internal server error.");
            }
        }
        [HttpGet]
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
                return StatusCode(500, "Internal server error.");
            }
        }
        [HttpGet("{moduleId}")]
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
                return StatusCode(500, "Internal server error.");
            }
        }
        [HttpGet("get/{moduleId}")]
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
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
