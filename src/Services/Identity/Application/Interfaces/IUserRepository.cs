using Codemy.Identity.Domain.Entities;

namespace Codemy.Identity.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task UpdateAsync(User user);
    }
}
