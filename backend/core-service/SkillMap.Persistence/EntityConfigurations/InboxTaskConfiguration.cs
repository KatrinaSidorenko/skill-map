using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SkillMap.Core.Tasks;

namespace SkillMap.Persistence.EntityConfigurations;
public class InboxTaskConfiguration : IEntityTypeConfiguration<InboxTask>
{
    public void Configure(EntityTypeBuilder<InboxTask> builder)
    {
        builder.ToTable("inbox_task");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired(false);

        builder.Property(x => x.Input)
            .HasColumnName("input")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.TaskType)
            .HasColumnName("task_type")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.WorkerId)
            .HasColumnName("worker_id")
            .IsRequired(false);

        builder.Property(x => x.Output)
            .HasColumnName("output")
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_inbox_task_status");
    }
}