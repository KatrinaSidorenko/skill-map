using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SkillMap.Core.RoadmapAssessments;

namespace SkillMap.Persistence.EntityConfigurations;

internal class RoadmapAssessmentConfiguration : IEntityTypeConfiguration<RoadmapAssessment>
{
    public void Configure(EntityTypeBuilder<RoadmapAssessment> builder)
    {
        builder.ToTable("roadmap_assessment");

        builder.HasKey(urt => urt.Id);
        builder.Property(urt => urt.Id).HasColumnName("id");

        builder.Property(urt => urt.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(urt => urt.UpdatedAt).HasColumnName("updated_at");

        builder.Property(urt => urt.RoadmapWorkspaceId).HasColumnName("roadmap_workspace_id").IsRequired();

        builder.Property(urt => urt.TestType)
            .HasColumnName("test_type")
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(urt => urt.TestData).HasColumnName("test_data").IsRequired();
        builder.HasOne(urt => urt.RoadmapFork)
            .WithMany(ur => ur.Assessments)
            .HasForeignKey(urt => urt.RoadmapWorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(urt => urt.Attempts)
            .WithOne(aa => aa.RoadmapAssessment)
            .HasForeignKey(aa => aa.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(urt => new { urt.RoadmapWorkspaceId, urt.TestType });
    }
}