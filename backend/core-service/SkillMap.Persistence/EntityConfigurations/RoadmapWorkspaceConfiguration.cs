using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Persistence.EntityConfigurations;

public class RoadmapWorkspaceConfiguration : IEntityTypeConfiguration<RoadmapWorkspace>
{
    public void Configure(EntityTypeBuilder<RoadmapWorkspace> builder)
    {
        builder.ToTable("roadmap_workspace");
        builder.HasKey(ur => ur.Id);
        builder.Property(ur => ur.Id).HasColumnName("id");
        builder.Property(rm => rm.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(rm => rm.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.Property(ur => ur.AuthorId).HasColumnName("author_id").IsRequired();
        builder.Property(ur => ur.RoadmapId).HasColumnName("roadmap_id");
        builder.Property(ur => ur.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);
        builder.Property(ur => ur.IsInAuthorMode).HasColumnName("is_in_author_mode").IsRequired().HasDefaultValue(false);
        builder.Property(ur => ur.PersonalRoadmapId).HasColumnName("personal_roadmap_id");

        builder.HasOne(ur => ur.Author)
            .WithMany(u => u.RoadmapForks)
            .HasForeignKey(ur => ur.AuthorId);

        builder.HasMany(ur => ur.WorkspaceEvents)
            .WithOne(rm => rm.RoadmapFork)
            .HasForeignKey(rm => rm.RoadmapWorkspaceId);

        builder.HasMany(ur => ur.Snapshots)
            .WithOne(rm => rm.RoadmapWorkspace)
            .HasForeignKey(rm => rm.RoadmapWorkspaceId);

        builder.HasOne(ur => ur.PersonalRoadmap)
            .WithOne(pr => pr.RoadmapWorkspace)
            .HasForeignKey<RoadmapWorkspace>(ur => ur.PersonalRoadmapId);

        builder.HasIndex(ur => new { ur.AuthorId, ur.RoadmapId });
    }
}