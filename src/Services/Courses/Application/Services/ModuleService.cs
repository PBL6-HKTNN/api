using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Application.Interfaces;
using Codemy.Courses.Domain.Entities;
using Codemy.IdentityProto;
using Microsoft.Extensions.Logging;

namespace Codemy.Courses.Application.Services
{
    internal class ModuleService : IModuleService
    {
        private readonly ILogger<ModuleService> _logger;
        private readonly IRepository<Module> _moduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IdentityService.IdentityServiceClient _client;
        public ModuleService(
            ILogger<ModuleService> logger,
            IRepository<Module> moduleRepository,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _moduleRepository = moduleRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<ModuleReponse> CreateModuleAsync(CreateModuleRequest request)
        {
            //CREATED BY, UPDATED BY
            //check update by giống instructorid của course mới đc
            //check sao k lưu đc updatedAt
            //check k trùng order trong cùng 1 course
            var module = new Module
            {
                Id = Guid.NewGuid(),
                title = request.title,
                order = request.order,
                courseId = request.courseId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _moduleRepository.AddAsync(module);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result <= 0)
            {
                return new ModuleReponse
                {
                    Success = false,
                    Message = "Failed to create module."
                };
            }
            return new ModuleReponse
            {
                Success = true,
                Message = "Module created successfully.",
                Module = module
            };
        }

        public async Task<ModuleListResponse> GetModules()
        {
            try
            {
                var module = await _moduleRepository.GetAllAsync();
                return new ModuleListResponse
                {
                    Success = true,
                    Message = "Get list module successfully.",
                    Modules = module.ToList()
                };
            }
            catch (Exception e)
            {
                return new ModuleListResponse
                {
                    Success = false,
                    Message = e.Message,
                    Modules = new List<Module>()
                };
            }
        }
    }
}
