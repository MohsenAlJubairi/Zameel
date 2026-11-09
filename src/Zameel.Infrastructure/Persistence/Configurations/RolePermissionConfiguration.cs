using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("rolePermissions");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.roleId).HasColumnName("roleId").IsRequired();
        builder.Property(x => x.permissionId).HasColumnName("permissionId").IsRequired();
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        // Relationships
        builder.HasOne(x => x.role)
            .WithMany(x => x.rolePermissions)
            .HasForeignKey(x => x.roleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.permission)
            .WithMany(x => x.rolePermissions)
            .HasForeignKey(x => x.permissionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => new { x.roleId, x.permissionId }).IsUnique();
    }
}
