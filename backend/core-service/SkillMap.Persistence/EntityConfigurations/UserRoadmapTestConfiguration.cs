using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SkillMap.Core.Entities.UserRoadmapTest;

namespace SkillMap.Persistence.EntityConfigurations;

internal class UserRoadmapTestConfiguration : IEntityTypeConfiguration<UserRoadmapTest>
{
    public void Configure(EntityTypeBuilder<UserRoadmapTest> builder)
    {
        builder.ToTable("user_roadmap_tests");

        builder.HasKey(urt => urt.Id);

        builder.Property(urt => urt.CreatedAt).IsRequired();
        builder.Property(urt => urt.UpdatedAt).IsRequired();

        builder.Property(urt => urt.UserRoadmapId).IsRequired();

        builder.Property(urt => urt.TestType)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(urt => urt.TestData).IsRequired();
        builder.HasOne(urt => urt.UserRoadmap)
            .WithMany(ur => ur.UserRoadmapTests)
            .HasForeignKey(urt => urt.UserRoadmapId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(urt => new { urt.UserRoadmapId, urt.TestType });
    }
}