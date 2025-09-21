using Microsoft.EntityFrameworkCore;
using SkillMap.Core;
using SkillMap.Core.Entities;

namespace SkillMap.Persistence;

public class SkillMapDbContext : DbContext
{
    public SkillMapDbContext(DbContextOptions<SkillMapDbContext> options) : base(options)
    {
    }

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<UserRoadmap> UserRoadmaps { get; set; }
    public DbSet<RoadmapModification> RoadmapModifications { get; set; }
    public DbSet<RoadmapSnapshot> RoadmapSnapshots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SkillMapDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker.Entries()
            .Where(e => e.Entity is TrackedEntity && (e.State == EntityState.Added || e.State == EntityState.Modified))
            .ToList();

        foreach (var entity in entities)
        {
            var trackingEntity = (TrackedEntity)entity.Entity;

            if (entity.State == EntityState.Added)
            {
                trackingEntity.CreatedAt = DateTime.UtcNow;
                trackingEntity.UpdatedAt = DateTime.UtcNow;
            }

            if (entity.State == EntityState.Modified)
            {
                trackingEntity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
