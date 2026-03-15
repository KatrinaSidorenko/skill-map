using System;
using System.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SkillMap.Core.Constants;
using SkillMap.Core.PersonalizedRoadmaps;

namespace SkillMap.Persistence.EntityConfigurations;
internal class RoadmapWorkspaceEventConfiguration : IEntityTypeConfiguration<RoadmapWorkspaceEvent>
{
    public void Configure(EntityTypeBuilder<RoadmapWorkspaceEvent> builder)
    {
        builder.ToTable("workspace_event");
        builder.HasKey(rm => rm.Id);
        builder.Property(rm => rm.Id).HasColumnName("id");
        builder.Property(rm => rm.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(rm => rm.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.Property(rm => rm.RoadmapWorkspaceId).HasColumnName("roadmap_workspace_id").IsRequired();
        builder.Property(rm => rm.EventType)
            .HasColumnName("event_type")
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (EventType)Enum.Parse(typeof(EventType), v));

        builder.Property(ws => ws.Version).HasColumnName("version").IsRequired();
        builder.HasIndex(rm => new { rm.RoadmapWorkspaceId, rm.Version }).IsUnique();

        builder.Property(ws => ws.Metadata).HasColumnType("jsonb").HasColumnName("metadata");
        builder.HasOne(rm => rm.RoadmapFork)
            .WithMany(ur => ur.WorkspaceEvents)
            .HasForeignKey(rm => rm.RoadmapWorkspaceId);

        builder.HasIndex(rm => new { rm.RoadmapWorkspaceId, rm.EventType });
    }
}