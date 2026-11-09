using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class StoreProfileConfiguration : IEntityTypeConfiguration<StoreProfile>
{
    public void Configure(EntityTypeBuilder<StoreProfile> builder)
    {
        builder.ToTable("storeProfiles");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.userId).HasColumnName("userId").IsRequired();
        builder.Property(x => x.storeName).HasColumnName("storeName").HasMaxLength(150).IsRequired();
        builder.Property(x => x.description).HasColumnName("description").HasMaxLength(1000);
        builder.Property(x => x.commercialRegister).HasColumnName("commercialRegister").HasMaxLength(100);
        builder.Property(x => x.locationDescription).HasColumnName("locationDescription").HasMaxLength(300);
        builder.Property(x => x.logoUrl).HasColumnName("logoUrl").HasMaxLength(500);
        builder.Property(x => x.subscriptionExpiresAt).HasColumnName("subscriptionExpiresAt").IsRequired();
        builder.Property(x => x.isApproved).HasColumnName("isApproved").HasDefaultValue(false);
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        builder.HasIndex(x => x.userId).IsUnique();
        builder.HasIndex(x => x.isApproved);
    }
}
