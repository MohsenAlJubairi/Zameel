using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("plans");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.description).HasColumnName("description").HasMaxLength(500);
        builder.Property(x => x.price).HasColumnName("price").HasColumnType("money").HasDefaultValue(0m);
        builder.Property(x => x.maxActiveListings).HasColumnName("maxActiveListings").HasDefaultValue(3);
        builder.Property(x => x.durationDays).HasColumnName("durationDays").HasDefaultValue(30);
        builder.Property(x => x.hasVerifiedBadge).HasColumnName("hasVerifiedBadge").HasDefaultValue(false);
        builder.Property(x => x.hasStoreProfile).HasColumnName("hasStoreProfile").HasDefaultValue(false);
        builder.Property(x => x.isDefault).HasColumnName("isDefault").HasDefaultValue(false);
        builder.Property(x => x.isActive).HasColumnName("isActive").HasDefaultValue(true);
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        // Relationships
        builder.HasMany(x => x.users)
            .WithOne(x => x.plan)
            .HasForeignKey(x => x.planId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
