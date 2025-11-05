using Codemy.BuildingBlocks.Core;
using Codemy.BuildingBlocks.Domain;
using Codemy.Courses.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking; 
namespace Codemy.Courses.Infrastructure.Persistence
{
    public class CourseDbContext : DbContext
    {
        private readonly IEnumerable<Type> _entityTypes;

        public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options)
        {
            // Get all types from Domain assembly that inherit from BaseEntity
            _entityTypes = typeof(BaseEntity).Assembly
                .GetTypes()
                .Where(t => t is { IsAbstract: false, IsClass: true } && t.IsSubclassOf(typeof(BaseEntity)));
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Register all entities
            foreach (var entityType in _entityTypes)
            {
                modelBuilder.Entity(entityType);
            }

            // Apply configurations (including seed data) 
        }

        public DbSet<T> GetDbSet<T>() where T : class, IAuditableEntity
        {
            return Set<T>();
        }

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
                            entry.Entity.Id = Guid.NewGuid();
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