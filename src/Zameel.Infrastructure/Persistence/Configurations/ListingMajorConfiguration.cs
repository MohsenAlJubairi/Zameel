using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class ListingMajorConfiguration : IEntityTypeConfiguration<ListingMajor>
{
    public void Configure(EntityTypeBuilder<ListingMajor> builder)
    {
        builder.ToTable("listingMajors");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.listingId).HasColumnName("listingId").IsRequired();
        builder.Property(x => x.majorId).HasColumnName("majorId").IsRequired();
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        builder.HasOne(x => x.listing)
            .WithMany(x => x.targetMajors)
            .HasForeignKey(x => x.listingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.major)
            .WithMany(x => x.listingMajors)
            .HasForeignKey(x => x.majorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.listingId, x.majorId }).IsUnique();
        builder.HasIndex(x => x.majorId);
    }
}
