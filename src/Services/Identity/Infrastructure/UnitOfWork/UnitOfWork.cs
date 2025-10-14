using Codemy.BuildingBlocks.Core; 
using Codemy.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Codemy.Identity.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IdentityDbContext _context;  // ← Must be IdentityDbContext, NOT ApplicationDbContext
        private IDbContextTransaction? _transaction;

        public UnitOfWork(IdentityDbContext context)  // ← Must be IdentityDbContext
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
