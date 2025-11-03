
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Domain.Entities;
using System.Reflection;
using Module = Codemy.Courses.Domain.Entities.Module;

namespace Codemy.Courses.Application.Interfaces 
{
    public interface IModuleService
    {
        Task<ModuleResponse> CreateModuleAsync(CreateModuleRequest request);
        Task<ModuleResponse> DeleteModuleAsync(Guid moduleId);
        Task<LessonListResponse> GetLessonByModuleId(Guid moduleId);
        Task<ModuleResponse> GetModuleById(Guid moduleId);
        Task<ModuleListResponse> GetModules();
        Task<ModuleResponse> UpdateModuleAsync(Guid moduleId, CreateModuleRequest request);
    }
    
    public class ModuleResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Module? Module { get; set; }
    }

    public class ModuleListResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<Module> Modules { get; set; } = new List<Module>();
    }
}
