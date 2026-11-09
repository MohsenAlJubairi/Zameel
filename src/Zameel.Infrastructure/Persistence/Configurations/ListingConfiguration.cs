using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
    public void Configure(EntityTypeBuilder<Listing> builder)
    {
        builder.ToTable("listings");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.userId).HasColumnName("userId").IsRequired();
        builder.Property(x => x.universityId).HasColumnName("universityId").IsRequired();
        builder.Property(x => x.primaryFacultyId).HasColumnName("primaryFacultyId");
        builder.Property(x => x.primaryMajorId).HasColumnName("primaryMajorId");
        builder.Property(x => x.title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(x => x.description).HasColumnName("description").HasMaxLength(4000).IsRequired();
        builder.Property(x => x.price).HasColumnName("price").HasColumnType("money").HasDefaultValue(0m);
        builder.Property(x => x.type).HasColumnName("type").HasConversion<int>().IsRequired();
        builder.Property(x => x.condition).HasColumnName("condition").HasConversion<int>().IsRequired();
        builder.Property(x => x.scope).HasColumnName("scope").HasConversion<int>().IsRequired();
        builder.Property(x => x.status).HasColumnName("status").HasConversion<int>().IsRequired();
        builder.Property(x => x.pickupStoreId).HasColumnName("pickupStoreId");
        builder.Property(x => x.reportedSoldCount).HasColumnName("reportedSoldCount").HasDefaultValue(0);
        builder.Property(x => x.viewsCount).HasColumnName("viewsCount").HasDefaultValue(0);
        builder.Property(x => x.expiresAt).HasColumnName("expiresAt").IsRequired();
        builder.Property(x => x.soldAt).HasColumnName("soldAt");
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        // Relationships
        builder.HasOne(x => x.user)
            .WithMany(x => x.listings)
            .HasForeignKey(x => x.userId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.university)
            .WithMany(x => x.listings)
            .HasForeignKey(x => x.universityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.primaryFaculty)
            .WithMany(x => x.primaryListings)
            .HasForeignKey(x => x.primaryFacultyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.primaryMajor)
            .WithMany(x => x.primaryListings)
            .HasForeignKey(x => x.primaryMajorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.images)
            .WithOne(x => x.listing)
            .HasForeignKey(x => x.listingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.targetFaculties)
            .WithOne(x => x.listing)
            .HasForeignKey(x => x.listingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.targetMajors)
            .WithOne(x => x.listing)
            .HasForeignKey(x => x.listingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.comments)
            .WithOne(x => x.listing)
            .HasForeignKey(x => x.listingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.reports)
            .WithOne(x => x.listing)
            .HasForeignKey(x => x.listingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.reviews)
            .WithOne(x => x.listing)
            .HasForeignKey(x => x.listingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.pickupStore)
            .WithMany(x => x.hostedListings)
            .HasForeignKey(x => x.pickupStoreId)
            .OnDelete(DeleteBehavior.Restrict);

        // High-Performance Composite Indexes for Campus Browsing
        builder.HasIndex(x => new { x.universityId, x.status, x.createdAt });
        builder.HasIndex(x => new { x.universityId, x.primaryFacultyId, x.status, x.createdAt });
        builder.HasIndex(x => new { x.universityId, x.primaryFacultyId, x.primaryMajorId, x.status, x.createdAt });
        builder.HasIndex(x => new { x.primaryMajorId, x.status });
        builder.HasIndex(x => new { x.userId, x.status });
        builder.HasIndex(x => new { x.status, x.expiresAt });
        builder.HasIndex(x => new { x.pickupStoreId, x.status });
    }
}
