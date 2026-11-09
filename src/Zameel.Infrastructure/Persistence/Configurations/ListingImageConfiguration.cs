using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class ListingImageConfiguration : IEntityTypeConfiguration<ListingImage>
{
    public void Configure(EntityTypeBuilder<ListingImage> builder)
    {
        builder.ToTable("listingImages");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.listingId).HasColumnName("listingId").IsRequired();
        builder.Property(x => x.imageUrl).HasColumnName("imageUrl").HasMaxLength(500).IsRequired();
        builder.Property(x => x.displayOrder).HasColumnName("displayOrder").HasDefaultValue(0);
        builder.Property(x => x.isPrimary).HasColumnName("isPrimary").HasDefaultValue(false);
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        builder.HasIndex(x => new { x.listingId, x.isPrimary });
    }
}
