using Codemy.Review.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ReviewEntity = Codemy.Review.Domain.Entities.Review;

namespace Codemy.Review.Infrastructure.Persistence
{
    public class ReviewDbContext : DbContext
    {
        public ReviewDbContext(DbContextOptions<ReviewDbContext> options) : base(options) { }

        public DbSet<ReviewEntity> Reviews { get; set; }
    }
}
