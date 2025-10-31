using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Domain;
using Codemy.Courses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace Codemy.Courses.Infrastructure.Persistence
{
    public class CourseDbContext : DbContext
    {
        private readonly IEnumerable<Type> _entityTypes;

        public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options)
        {
            _entityTypes = typeof(Course).Assembly
                .GetTypes()
                .Where(t => t is { IsAbstract: false, IsClass: true } && t.IsSubclassOf(typeof(BaseEntity)));
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Register all entities
            foreach (var entityType in _entityTypes)
            {
                modelBuilder.Entity(entityType);
            }

            // Example: configure relationships if needed
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Course)
                      .WithMany()
                      .HasForeignKey(e => e.CourseId);
            });
        }

        public DbSet<T> GetDbSet<T>() where T : class, IAuditableEntity
        {
            return Set<T>();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedAt = null;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (entry.Entity.Id == Guid.Empty)
                            entry.Entity.Id = Guid.NewGuid();
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
