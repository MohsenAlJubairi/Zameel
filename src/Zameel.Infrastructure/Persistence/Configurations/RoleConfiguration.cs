using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.normalizedName).HasColumnName("normalizedName").HasMaxLength(100).IsRequired();
        builder.Property(x => x.description).HasColumnName("description").HasMaxLength(500);
        builder.Property(x => x.isSystemRole).HasColumnName("isSystemRole").HasDefaultValue(false);
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        // Relationships
        builder.HasMany(x => x.users)
            .WithOne(x => x.role)
            .HasForeignKey(x => x.roleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.rolePermissions)
            .WithOne(x => x.role)
            .HasForeignKey(x => x.roleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.normalizedName).IsUnique();
    }
}
