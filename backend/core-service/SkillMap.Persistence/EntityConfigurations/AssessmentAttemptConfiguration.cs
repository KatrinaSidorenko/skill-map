using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SkillMap.Core.RoadmapAssessments;

namespace SkillMap.Persistence.EntityConfigurations;
internal class AssessmentAttemptConfiguration : IEntityTypeConfiguration<AssessmentAttempt>
{
    public void Configure(EntityTypeBuilder<AssessmentAttempt> builder)
    {
        builder.ToTable("assessment_attempt");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(utr => utr.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(utr => utr.UpdatedAt).HasColumnName("updated_at").IsRequired(false);

        builder.Property(utr => utr.AssessmentId).HasColumnName("assessment_id").IsRequired();

        builder.Property(utr => utr.MaxPoints)
            .HasColumnName("max_points")
            .IsRequired();

        builder.Property(utr => utr.ScoredPoints)
            .HasColumnName("scored_points")
            .IsRequired();

        builder.Property(utr => utr.StartedAt).HasColumnName("started_at").IsRequired();
        builder.Property(utr => utr.CompletedAt).HasColumnName("completed_at");

        builder.Property(utr => utr.ResultData).HasColumnName("result_data").IsRequired(false);
        builder.HasOne(utr => utr.RoadmapAssessment)
            .WithMany(ra => ra.Attempts)
            .HasForeignKey(utr => utr.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(utr => new { utr.AssessmentId, utr.CompletedAt });
    }
}