using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class MajorConfiguration : IEntityTypeConfiguration<Major>
{
    public void Configure(EntityTypeBuilder<Major> builder)
    {
        builder.ToTable("majors");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(x => x.slug).HasColumnName("slug").HasMaxLength(150).IsRequired();
        builder.Property(x => x.isActive).HasColumnName("isActive").HasDefaultValue(true);
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        builder.HasMany(x => x.users)
            .WithOne(x => x.major)
            .HasForeignKey(x => x.majorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.primaryListings)
            .WithOne(x => x.primaryMajor)
            .HasForeignKey(x => x.primaryMajorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.slug).IsUnique();
        builder.HasIndex(x => x.name);
    }
}
