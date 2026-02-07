using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SkillMap.Core.Constants;
using SkillMap.Core.Entities;

namespace SkillMap.Persistence.EntityConfigurations;
internal class RoadmapModificationConfiguration : IEntityTypeConfiguration<RoadmapModification>
{
    public void Configure(EntityTypeBuilder<RoadmapModification> builder)
    {
        builder.ToTable("roadmap_modifications");
        builder.HasKey(rm => rm.Id);
        builder.Property(rm => rm.CreatedAt).IsRequired();
        builder.Property(rm => rm.UpdatedAt).IsRequired();

        builder.Property(rm => rm.UserRoadmapId).IsRequired();
        builder.Property(rm => rm.InnerItemId);
        builder.Property(rm => rm.ExternalItemId);
        builder.Property(rm => rm.Action)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (ModificationAction)Enum.Parse(typeof(ModificationAction), v));
        builder.HasOne(rm => rm.UserRoadmap)
            .WithMany(ur => ur.RoadmapModifications)
            .HasForeignKey(rm => rm.UserRoadmapId);
        builder.HasIndex(rm => new { rm.UserRoadmapId, rm.InnerItemId, rm.ExternalItemId });
    }
}