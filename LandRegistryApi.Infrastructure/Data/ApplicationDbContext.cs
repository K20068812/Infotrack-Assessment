using LandRegistryApi.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LandRegistryApi.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SearchResult>(entity =>
            {
                entity.HasKey(sr => sr.Id);
                entity.Property(sr => sr.SearchQuery).IsRequired().HasMaxLength(500);
                entity.Property(sr => sr.TargetUrl).IsRequired().HasMaxLength(500);
                entity.Property(sr => sr.Positions).IsRequired().HasMaxLength(1000);

                // Index for faster queries on target URL and search date
                entity.HasIndex(sr => new { sr.TargetUrl, sr.SearchDate });
            });
        }

        public DbSet<SearchResult> SearchResults { get; set; }
    }
}
