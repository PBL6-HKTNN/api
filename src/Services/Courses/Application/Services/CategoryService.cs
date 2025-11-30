using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Codemy.Courses.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
namespace Codemy.Courses.Application.Services
{
    internal class CategoryService : ICategoryService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CategoryService> _logger;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork; 
        public CategoryService(
            IHttpContextAccessor httpContextAccessor,
            ILogger<CategoryService> logger,
            IRepository<Category> categoryRepository,
            IUnitOfWork unitOfWork )
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<CategoryReponse> CreateCategoryAsync(CreateCategoryRequest request)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                return new CategoryReponse
                {
                    Success = false,
                    Message = "User not authenticated or token missing."
                };
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            var userId = Guid.Parse(userIdClaim);
            
            var categorys = await _categoryRepository.FindAsync(c => c.name == request.Name && !c.IsDeleted);
            if (categorys.Any())
            {
                return new CategoryReponse
                {
                    Success = false,
                    Message = "Category with the same name already exists."
                };
            }
            var category = new Category
            {
                Id = Guid.NewGuid(),
                name = request.Name,
                description = request.Description,
                CreatedBy = userId,
            };
            await _categoryRepository.AddAsync(category);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new CategoryReponse
                {
                    Success = false,
                    Message = "Failed to create category."
                };
            }
            return new CategoryReponse
            {
                Success = true,
                Message = "Category created successfully.",
                Category = category
            };
        }

        public async Task<CategoryReponse> DeleteCategoryById(Guid categoryId)
        {
            var categorys = await _categoryRepository.FindAsync(c => c.Id == categoryId && !c.IsDeleted);
            if (categorys.Count == 0)
            {
                return new CategoryReponse
                {
                    Success = false,
                    Message = "Category not found."
                };
            }
            var category = categorys.First();
            _categoryRepository.Delete(category);
            var result =  await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new CategoryReponse
                {
                    Success = false,
                    Message = "Failed to delete category."
                };
            }
            return new CategoryReponse
            {
                Success = true,
                Message = "Category deleted successfully.",
                Category = category
            };
        }

        public async Task<CategoryListResponse> GetCategories()
        {
            try
            {
                var category = await _categoryRepository.GetAllAsync();
                var categoryFiltered = category.Where(c => !c.IsDeleted);
                return new CategoryListResponse
                {
                    Success = true,
                    Message = "Get list category successfully.",
                    Categories = categoryFiltered.ToList()
                };
            }
            catch (Exception e)
            {
                return new CategoryListResponse
                {
                    Success = false,
                    Message = e.Message,
                    Categories = new List<Category>()
                };
            }
        }

        public async Task<CategoryReponse> GetCategoryById(Guid categoryId)
        {
            var category = await _categoryRepository.FindAsync(c => c.Id == categoryId && !c.IsDeleted);
            if (category.Count == 0)
            {
                return new CategoryReponse
                {
                    Success = false,
                    Message = "Category not found."
                };
            }
            return new CategoryReponse
            {
                Success = true,
                Message = "Category retrieved successfully.",
                Category =  category.First()
            };
        }

        public async Task<CategoryReponse> UpdateCategoryById(Guid categoryId, CreateCategoryRequest request)
        {
            var categorys = await _categoryRepository.FindAsync(c => c.Id == categoryId && !c.IsDeleted);
            if (categorys.Count == 0)
            {
                return new CategoryReponse
                {
                    Success = false,
                    Message = "Category not found."
                };
            }

            var categoryNameExists = await _categoryRepository
                .FindAsync(c => c.name == request.Name && c.Id != categoryId);
            if (categoryNameExists.Any())
            {
                return new CategoryReponse
                {
                    Success = false,
                    Message = "Another category with the same name already exists."
                };
            }   
            var category = categorys.First();
            category.name = request.Name;
            category.description = request.Description;
            _categoryRepository.Update(category);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new CategoryReponse
                {
                    Success = false,
                    Message = "Failed to update category."
                };
            }
            return new CategoryReponse
            {
                Success = true,
                Message = "Category updated successfully.",
                Category = category
            };
        }
    }
}
