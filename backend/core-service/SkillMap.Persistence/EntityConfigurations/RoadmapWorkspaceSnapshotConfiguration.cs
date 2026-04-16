using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SkillMap.Core.RoadmapsWorkspace.RoadmapSnapshots;

namespace SkillMap.Persistence.EntityConfigurations;

public class RoadmapWorkspaceSnapshotConfiguration : IEntityTypeConfiguration<RoadmapWorkspaceSnapshot>
{
    public void Configure(EntityTypeBuilder<RoadmapWorkspaceSnapshot> builder)
    {
        builder.ToTable("workspace_snapshot");
        builder.HasKey(rs => rs.Id);
        builder.Property(rs => rs.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(rs => rs.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(rs => rs.UpdatedAt).HasColumnName("updated_at").IsRequired(false);
        builder.Property(rs => rs.RoadmapWorkspaceId).HasColumnName("roadmap_workspace_id").IsRequired();
        builder.Property(rs => rs.Version).HasColumnName("version").IsRequired();
        builder.Property(rs => rs.Content).HasColumnName("content");
        builder.HasIndex(rm => new { rm.RoadmapWorkspaceId, rm.Version }).IsUnique();

        builder.HasOne(rs => rs.RoadmapWorkspace)
            .WithMany(ur => ur.Snapshots)
            .HasForeignKey(rs => rs.RoadmapWorkspaceId);

        builder.HasIndex(rs => new { rs.RoadmapWorkspaceId, rs.CreatedAt });
    }
}