using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.listingId).HasColumnName("listingId").IsRequired();
        builder.Property(x => x.reviewerId).HasColumnName("reviewerId").IsRequired();
        builder.Property(x => x.revieweeId).HasColumnName("revieweeId").IsRequired();
        builder.Property(x => x.rating).HasColumnName("rating").IsRequired();
        builder.Property(x => x.comment).HasColumnName("comment").HasMaxLength(1000);
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        // One review per reviewer per listing
        builder.HasIndex(x => new { x.listingId, x.reviewerId }).IsUnique();
        builder.HasIndex(x => new { x.revieweeId, x.rating });
    }
}
