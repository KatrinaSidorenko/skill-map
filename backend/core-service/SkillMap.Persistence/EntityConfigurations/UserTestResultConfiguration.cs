using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SkillMap.Core.Entities.UserRoadmapTest;

namespace SkillMap.Persistence.EntityConfigurations;
internal class UserTestResultConfiguration : IEntityTypeConfiguration<UserTestResult>
{
    public void Configure(EntityTypeBuilder<UserTestResult> builder)
    {
        builder.ToTable("user_test_results");

        builder.HasKey(utr => utr.Id);

        builder.Property(utr => utr.CreatedAt).IsRequired();
        builder.Property(utr => utr.UpdatedAt).IsRequired();

        builder.Property(utr => utr.UserRoadmapTestId).IsRequired();

        builder.Property(utr => utr.MaxPoints)
            .IsRequired();

        builder.Property(utr => utr.ScoredPoints)
            .IsRequired();

        builder.Property(utr => utr.StartedAt).IsRequired();
        builder.Property(utr => utr.CompletedAt);

        builder.Property(utr => utr.ResultData).IsRequired(false);
        builder.HasOne(utr => utr.UserRoadmapTest)
            .WithMany()
            .HasForeignKey(utr => utr.UserRoadmapTestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(utr => new { utr.UserRoadmapTestId, utr.CompletedAt });
    }
}