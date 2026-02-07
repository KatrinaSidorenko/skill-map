using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using SkillMap.Core.Entities;

namespace SkillMap.Persistence.EntityConfigurations;

internal class UserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);
        builder.Property(rm => rm.CreatedAt).IsRequired();
        builder.Property(rm => rm.UpdatedAt).IsRequired();

        builder.Property(u => u.UserName).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Role).IsRequired().HasMaxLength(20);

        builder.HasIndex(u => u.Email).IsUnique();
    }
}