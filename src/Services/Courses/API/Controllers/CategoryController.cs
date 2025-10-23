using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Application.DTOs; 
using Codemy.Courses.Application.Interfaces;
using Microsoft.AspNetCore.Cors.Infrastructure; 
using Microsoft.AspNetCore.Mvc;
namespace Codemy.Courses.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;
        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
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
                var result = await _categoryService.CreateCategoryAsync(request);
                if (!result.Success) {
                    return this.BadRequestResponse(
                        result.Message ?? "Failed to create category.",
                        "Category creation failed due to business logic constraints."
                    );
                }
                return this.CreatedResponse(
                    result.Category,
                    $"/category/get/{result.Category.Id}"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category.");
                return StatusCode(500, "Internal server error.");
            }
        }
        [HttpGet] 
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var result = await _categoryService.GetCategories();
                if (result.Categories.Count == 0)
                {
                    return this.NotFoundResponse( "Categories not found.",
                        "No categories available in the system."
                    );
                }
                return this.OkResponse(result.Categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error get category.");
                return StatusCode(500, "Internal server error.");
            }
        }

    }
}
