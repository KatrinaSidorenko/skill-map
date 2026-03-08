using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SkillMap.Core.RoadmapBookmarks;

namespace SkillMap.Persistence.EntityConfigurations;

public class RoadmapForkConfiguration : IEntityTypeConfiguration<RoadmapFork>
{
    public void Configure(EntityTypeBuilder<RoadmapFork> builder)
    {
        builder.ToTable("roadmap_fork");
        builder.HasKey(ur => ur.Id);
        builder.Property(ur => ur.Id).HasColumnName("id");
        builder.Property(rm => rm.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(rm => rm.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.Property(ur => ur.AuthorId).HasColumnName("author_id").IsRequired();
        builder.Property(ur => ur.RoadmapId).HasColumnName("roadmap_id").IsRequired();
        builder.Property(ur => ur.IsActive).HasColumnName("is_active").IsRequired().HasDefaultValue(true);

        builder.HasOne(ur => ur.User)
            .WithMany(u => u.RoadmapForks)
            .HasForeignKey(ur => ur.AuthorId);

        builder.HasMany(ur => ur.WorkspaceEvents)
            .WithOne(rm => rm.RoadmapFork)
            .HasForeignKey(rm => rm.RoadmapForkId);

        builder.HasMany(ur => ur.Snapshots)
            .WithOne(rm => rm.RoadmapFork)
            .HasForeignKey(rm => rm.RoadmapForkId);

        builder.HasIndex(ur => new { ur.AuthorId, ur.RoadmapId });
    }
}