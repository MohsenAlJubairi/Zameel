using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.systemName).HasColumnName("systemName").HasMaxLength(100).IsRequired();
        builder.Property(x => x.displayName).HasColumnName("displayName").HasMaxLength(150).IsRequired();
        builder.Property(x => x.module).HasColumnName("module").HasMaxLength(50).IsRequired();
        builder.Property(x => x.description).HasColumnName("description").HasMaxLength(500);
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        // Relationships
        builder.HasMany(x => x.rolePermissions)
            .WithOne(x => x.permission)
            .HasForeignKey(x => x.permissionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.systemName).IsUnique();
    }
}
