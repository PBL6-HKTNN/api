using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Domain.Entities;

namespace Codemy.Courses.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request);
        Task<CategoryListResponse> GetCategories();
    }
    public class CategoryResponse
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

