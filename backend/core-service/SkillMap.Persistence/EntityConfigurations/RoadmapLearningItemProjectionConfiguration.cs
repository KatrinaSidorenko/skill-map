using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SkillMap.Core.RoadmapsWorkspace;

namespace SkillMap.Persistence.EntityConfigurations;
internal class RoadmapLearningItemProjectionConfiguration : IEntityTypeConfiguration<RoadmapLearningProjection>
{
    public void Configure(EntityTypeBuilder<RoadmapLearningProjection> builder)
    {
        builder.ToTable("roadmap_learning_item_projection");
        builder.HasKey(rlis => rlis.Id);
        builder.Property(rlis => rlis.Id).HasColumnName("id");
        builder.Property(rlis => rlis.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(rlis => rlis.UpdatedAt).HasColumnName("updated_at");
        builder.Property(rlis => rlis.RoadmapWorkspaceId).HasColumnName("roadmap_workspace_id").IsRequired();
        builder.Property(rlis => rlis.LearningItemId).HasColumnName("learning_item_id").IsRequired().HasMaxLength(100);
        builder.Property(rlis => rlis.Status).HasColumnName("status").IsRequired().HasMaxLength(50);
        builder.Property(rlis => rlis.IsAvailable).HasColumnName("is_available").IsRequired();
        builder.HasOne(rlis => rlis.RoadmapWorkspace)
            .WithMany(rw => rw.LearningItemStatuses)
            .HasForeignKey(rlis => rlis.RoadmapWorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(rlis => new { rlis.RoadmapWorkspaceId, rlis.LearningItemId }).IsUnique();
        builder.HasIndex(rlis => new { rlis.RoadmapWorkspaceId, rlis.IsAvailable, rlis.Status });
    }
}
