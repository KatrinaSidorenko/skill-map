using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SkillMap.Core.Roadmaps;

namespace SkillMap.Persistence.EntityConfigurations;
public class PersonalRoadmapConfiguration : IEntityTypeConfiguration<PersonalRoadmap>
{
    public void Configure(EntityTypeBuilder<PersonalRoadmap> builder)
    {
        builder.ToTable("personal_roadmap");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired(false);
        builder.Property(x => x.AuthorId).HasColumnName("author_id").IsRequired();
        builder.Property(x => x.Title).HasColumnName("title").IsRequired().HasMaxLength(255);
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(1000);
        builder.Property(x => x.ImageUrl).HasColumnName("image_url").HasMaxLength(500);
        builder.Property(x => x.IsPublic).HasColumnName("is_public").IsRequired();

        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
