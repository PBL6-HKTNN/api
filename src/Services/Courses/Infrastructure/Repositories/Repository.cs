using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions; 

namespace Codemy.Courses.Infrastructure
{
    public class Repository<T> : IRepository<T> where T : class, IAuditableEntity
    {
        private readonly CourseDbContext _context;   
        private readonly DbSet<T> _dbSet;

        public Repository(CourseDbContext context)  
        {
            _context = context;
            _dbSet = context.GetDbSet<T>();
        }

        public IQueryable<T> TableNoTracking => _dbSet.AsNoTracking();

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
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
    }
}
