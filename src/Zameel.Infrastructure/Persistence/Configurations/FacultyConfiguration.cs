using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
{
    public void Configure(EntityTypeBuilder<Faculty> builder)
    {
        builder.ToTable("faculties");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(x => x.slug).HasColumnName("slug").HasMaxLength(150).IsRequired();
        builder.Property(x => x.isActive).HasColumnName("isActive").HasDefaultValue(true);
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        builder.HasMany(x => x.users)
            .WithOne(x => x.faculty)
            .HasForeignKey(x => x.facultyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.primaryListings)
            .WithOne(x => x.primaryFaculty)
            .HasForeignKey(x => x.primaryFacultyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.slug).IsUnique();
        builder.HasIndex(x => x.name);
    }
}
