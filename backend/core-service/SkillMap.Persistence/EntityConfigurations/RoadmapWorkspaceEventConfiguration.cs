using System;
using System.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SkillMap.Core.PersonalizedRoadmaps;
using SkillMap.Core.RoadmapsWorkspace.Events;

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
        builder.Property(rm => rm.IdempotencyKey).HasColumnName("idempotency_key").IsRequired();
        builder.Property(rm => rm.EventType)
            .HasColumnName("event_type")
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (WorkspaceEventType)Enum.Parse(typeof(WorkspaceEventType), v));

        builder.Property(ws => ws.Version).HasColumnName("version").IsRequired();

        builder.Property(ws => ws.Metadata).HasColumnType("jsonb").HasColumnName("metadata");
        builder.HasOne(rm => rm.RoadmapWorkspace)
            .WithMany(ur => ur.WorkspaceEvents)
            .HasForeignKey(rm => rm.RoadmapWorkspaceId);

        builder.HasIndex(rm => new { rm.RoadmapWorkspaceId, rm.Version }).IsUnique();
        builder.HasIndex(rm => new { rm.RoadmapWorkspaceId, rm.EventType });
        builder.HasIndex(rm => new { rm.RoadmapWorkspaceId, rm.IdempotencyKey });
    }
}