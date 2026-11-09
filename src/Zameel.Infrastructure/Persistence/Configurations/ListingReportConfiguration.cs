using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class ListingReportConfiguration : IEntityTypeConfiguration<ListingReport>
{
    public void Configure(EntityTypeBuilder<ListingReport> builder)
    {
        builder.ToTable("listingReports");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.listingId).HasColumnName("listingId").IsRequired();
        builder.Property(x => x.reporterId).HasColumnName("reporterId").IsRequired();
        builder.Property(x => x.reason).HasColumnName("reason").HasConversion<int>().IsRequired();
        builder.Property(x => x.notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(x => x.isProcessed).HasColumnName("isProcessed").HasDefaultValue(false);
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        // One report of a specific reason per user per listing
        builder.HasIndex(x => new { x.listingId, x.reporterId, x.reason }).IsUnique();
        builder.HasIndex(x => new { x.listingId, x.isProcessed });
    }
}
