using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class UniversityConfiguration : IEntityTypeConfiguration<University>
{
    public void Configure(EntityTypeBuilder<University> builder)
    {
        builder.ToTable("universities");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(x => x.city).HasColumnName("city").HasMaxLength(100).IsRequired();
        builder.Property(x => x.logoUrl).HasColumnName("logoUrl").HasMaxLength(500);
        builder.Property(x => x.isActive).HasColumnName("isActive").HasDefaultValue(true);
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        builder.HasMany(x => x.users)
            .WithOne(x => x.university)
            .HasForeignKey(x => x.universityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.listings)
            .WithOne(x => x.university)
            .HasForeignKey(x => x.universityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.name);
    }
}
