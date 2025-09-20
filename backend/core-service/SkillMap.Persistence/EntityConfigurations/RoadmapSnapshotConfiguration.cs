using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillMap.Core.Entities;

namespace SkillMap.Persistence.EntityConfigurations;

public class RoadmapSnapshotConfiguration : IEntityTypeConfiguration<RoadmapSnapshot>
{
    public void Configure(EntityTypeBuilder<RoadmapSnapshot> builder)
    {
        builder.ToTable("roadmap_snapshots");
        builder.HasKey(rs => rs.Id);
        builder.Property(rs => rs.Id).ValueGeneratedOnAdd();
        builder.Property(rs => rs.CreatedAt).IsRequired();
        builder.Property(rs => rs.UpdatedAt).IsRequired(false);
        builder.Property(rs => rs.UserRoadmapId).IsRequired();
       
        builder.Property(rs => rs.Content).IsRequired();

        builder.HasOne(rs => rs.UserRoadmap)
            .WithMany(ur => ur.RoadmapSnapshots)
            .HasForeignKey(rs => rs.UserRoadmapId);

        builder.HasIndex(rs => new { rs.UserRoadmapId, rs.CreatedAt });
    }
}
