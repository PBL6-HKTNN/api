using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Core.Extensions;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Codemy.Courses.Domain.Entities;
using Codemy.Identity.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codemy.IdentityProto;
namespace Codemy.Courses.Application.Services
{
    internal class CategoryService : ICategoryService
    {
        private readonly ILogger<CategoryService> _logger;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IUnitOfWork _unitOfWork; 
        public CategoryService(
            ILogger<CategoryService> logger,
            IRepository<Category> categoryRepository,
            IUnitOfWork unitOfWork )
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<CategoryReponse> CreateCategoryAsync(CreateCategoryRequest request)
        {
            //CREATED BY, UPDATED BY
            var categorys = await _categoryRepository.FindAsync(c => c.name == request.Name);
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
                description = request.Description
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

        public async Task<CategoryListResponse> GetCategories()
        {
            try
            {
                var category = await _categoryRepository.GetAllAsync();
                return new CategoryListResponse
                {
                    Success = true,
                    Message = "Get list category successfully.",
                    Categories = category.ToList()
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
    }
}
