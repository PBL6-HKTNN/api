using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codemy.Courses.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryReponse> CreateCategoryAsync(CreateCategoryRequest request);
        Task<CategoryListResponse> GetCategories();
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

