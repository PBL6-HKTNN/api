using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Domain;
using Codemy.Review.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ReviewEntity = Codemy.Review.Domain.Entities.Review;

namespace Codemy.Review.Infrastructure.Persistence
{
    public class ReviewDbContext : DbContext
    {
        public ReviewDbContext(DbContextOptions<ReviewDbContext> options) : base(options) { }

        public DbSet<ReviewEntity> Reviews { get; set; }

        // Override SaveChanges to handle audit properties automatically
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedAt = null;
                        // TODO: Set CreatedBy from current user context if available
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        // TODO: Set UpdatedBy from current user context if available
                        break;
                }
            }

            // Handle BaseEntity specifically for additional properties like Id generation and soft delete
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        // Generate new Guid if not already set
                        if (entry.Entity.Id == Guid.Empty)
                        {
                            entry.Entity.Id = Guid.Parse("44444444-4444-4444-4444-444444444440");
                        }
                        break;
                    case EntityState.Deleted:
                        // Implement soft delete
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        // TODO: Set DeletedBy from current user context if available
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
