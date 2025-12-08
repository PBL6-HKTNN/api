using Codemy.BuildingBlocks.Core;
using Codemy.Review.Application.Interfaces;
using Codemy.Review.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ReviewEntity = Codemy.Review.Domain.Entities.Review;

namespace Codemy.Review.Infrastructure.Repositories
{
    public class Repository : IRepository<ReviewEntity>, IReviewRepository
    {
        private readonly ReviewDbContext _context;
        private readonly DbSet<ReviewEntity> _dbSet;

        public Repository(ReviewDbContext context)
        {
            _context = context;
            _dbSet = context.Reviews;
        }

        #region IRepository<T> implementation (generic CRUD)

        public IQueryable<ReviewEntity> TableNoTracking => _dbSet.AsNoTracking();

        public async Task<IReadOnlyList<ReviewEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IReadOnlyList<ReviewEntity>> GetAllAsync(Expression<Func<ReviewEntity, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task AddAsync(ReviewEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(ReviewEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified; 
        }

        public async Task UpdateAsync(ReviewEntity entity)
        {
            _context.Set<ReviewEntity>().Update(entity);
        }

        public void Delete(ReviewEntity entity)
        {
            _dbSet.Remove(entity); 
        }

        public async Task<IReadOnlyList<ReviewEntity>> FindAsync(Expression<Func<ReviewEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<ReviewEntity?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public IQueryable<ReviewEntity> Query()
        {
            return _dbSet.AsQueryable();
        }

        #endregion

        #region IReviewRepository (custom methods)

        public async Task<IEnumerable<ReviewEntity>> GetByCourseIdAsync(Guid courseId)
        {
            return await _dbSet
                .Where(r => r.courseId == courseId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(Guid courseId)
        {
            var ratings = await _dbSet
                .Where(r => r.courseId == courseId)
                .Select(r => r.rating)
                .ToListAsync();

            return ratings.Count == 0 ? 0 : ratings.Average();
        }

        #endregion
    }
}
