using Microsoft.EntityFrameworkCore;

using SkillMap.Core;
using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapAssessments;
using SkillMap.Core.RoadmapsWorkspace;
using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;
using SkillMap.Core.User;

namespace SkillMap.Persistence;

public class SkillMapDbContext : DbContext
{
    public SkillMapDbContext(DbContextOptions<SkillMapDbContext> options) : base(options) { }

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<RoadmapWorkspace> RoadmapForks { get; set; }
    public DbSet<RoadmapWorkspaceEvent> RoadmapWorkspaceEvents { get; set; }
    public DbSet<RoadmapWorkspaceSnapshot> RoadmapWorkspaceSnapshots { get; set; }
    public DbSet<RoadmapAssessment> RoadmapAssessments { get; set; }
    public DbSet<AssessmentAttempt> AssessmentAttempts { get; set; }

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