using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace Codemy.BuildingBlocks.Infrastructure.Repositories
{
    internal class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : class, IAuditableEntity
    {
        private readonly DbSet<T> _dbSet = context.GetDbSet<T>();

        public IQueryable<T> TableNoTracking => _dbSet.AsNoTracking();

        public async Task<T?> GetByIdAsync(T id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
    }
}
