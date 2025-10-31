
using Codemy.Courses.Application.DTOs;
using Codemy.Courses.Domain.Entities;
using System.Reflection;
using Module = Codemy.Courses.Domain.Entities.Module;

namespace Codemy.Courses.Application.Interfaces 
{
    public interface IModuleService
    {
        Task<ModuleReponse> CreateModuleAsync(CreateModuleRequest request);
        Task<ModuleListResponse> GetModules();
    }
    
    public class ModuleReponse
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
