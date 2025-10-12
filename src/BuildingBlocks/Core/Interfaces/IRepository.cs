using System.Linq.Expressions;

namespace Codemy.BuildingBlocks.Core
{
    public interface IRepository<T> where T : class, IAuditableEntity
    {
        Task<T?> GetByIdAsync(object id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> TableNoTracking { get; }
    }
}
