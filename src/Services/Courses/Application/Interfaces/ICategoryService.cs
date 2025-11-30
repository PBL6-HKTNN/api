using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Domain.Entities;

namespace Codemy.Courses.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryReponse> CreateCategoryAsync(CreateCategoryRequest request);
        Task<CategoryReponse> DeleteCategoryById(Guid categoryId);
        Task<CategoryListResponse> GetCategories();
        Task<CategoryReponse> GetCategoryById(Guid categoryId);
        Task<CategoryReponse> UpdateCategoryById(Guid categoryId, CreateCategoryRequest request);
    }
    public class CategoryReponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Category? Category { get; set; }
    }
    public class CategoryListResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}

