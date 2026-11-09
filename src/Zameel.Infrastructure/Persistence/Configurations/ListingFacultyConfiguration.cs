using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class ListingFacultyConfiguration : IEntityTypeConfiguration<ListingFaculty>
{
    public void Configure(EntityTypeBuilder<ListingFaculty> builder)
    {
        builder.ToTable("listingFaculties");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.listingId).HasColumnName("listingId").IsRequired();
        builder.Property(x => x.facultyId).HasColumnName("facultyId").IsRequired();
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        builder.HasOne(x => x.listing)
            .WithMany(x => x.targetFaculties)
            .HasForeignKey(x => x.listingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.faculty)
            .WithMany(x => x.listingFaculties)
            .HasForeignKey(x => x.facultyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.listingId, x.facultyId }).IsUnique();
        builder.HasIndex(x => x.facultyId);
    }
}
