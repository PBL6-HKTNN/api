using Codemy.BuildingBlocks.Core;
using Codemy.Courses.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Codemy.Courses.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CourseDbContext _context;  
        private IDbContextTransaction? _transaction;

        public UnitOfWork(CourseDbContext context) 
        {
            _context = context;
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null) await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null) await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
