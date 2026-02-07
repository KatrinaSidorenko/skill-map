using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SkillMap.Core.Entities;

namespace SkillMap.Persistence.EntityConfigurations;

public class UserRoadmapConfiguration : IEntityTypeConfiguration<UserRoadmap>
{
    public void Configure(EntityTypeBuilder<UserRoadmap> builder)
    {
        builder.ToTable("user_roadmaps");
        builder.HasKey(ur => ur.Id);
        builder.Property(rm => rm.CreatedAt).IsRequired();
        builder.Property(rm => rm.UpdatedAt).IsRequired();

        builder.Property(ur => ur.UserId).IsRequired();
        builder.Property(ur => ur.RoadmapId).IsRequired();
        builder.Property(ur => ur.IsActive).IsRequired().HasDefaultValue(true);

        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoadmaps)
            .HasForeignKey(ur => ur.UserId);

        builder.HasMany(ur => ur.RoadmapModifications)
            .WithOne(rm => rm.UserRoadmap)
            .HasForeignKey(rm => rm.UserRoadmapId);

        builder.HasIndex(ur => new { ur.UserId, ur.RoadmapId });
    }
}