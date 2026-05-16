using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SkillMap.Core.User;

namespace SkillMap.Persistence.EntityConfigurations;

internal class UserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("user");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id");
        builder.Property(u => u.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at").IsRequired(false);

        builder.Property(u => u.UserName).HasColumnName("user_name").IsRequired().HasMaxLength(50);
        builder.Property(u => u.Email).HasColumnName("email").IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).HasColumnName("password_hash").IsRequired();
        builder.Property(u => u.Role).HasColumnName("role").IsRequired().HasMaxLength(20);
        builder.Property(u => u.ImageUrl).HasColumnName("image_url").IsRequired(false).HasMaxLength(500);

        builder.HasIndex(u => u.Email).IsUnique();
    }
}