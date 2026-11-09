using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class FacultyMajorConfiguration : IEntityTypeConfiguration<FacultyMajor>
{
    public void Configure(EntityTypeBuilder<FacultyMajor> builder)
    {
        builder.ToTable("facultyMajors");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.facultyId).HasColumnName("facultyId").IsRequired();
        builder.Property(x => x.majorId).HasColumnName("majorId").IsRequired();
        builder.Property(x => x.isActive).HasColumnName("isActive").HasDefaultValue(true);
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        builder.HasOne(x => x.faculty)
            .WithMany(x => x.facultyMajors)
            .HasForeignKey(x => x.facultyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.major)
            .WithMany(x => x.facultyMajors)
            .HasForeignKey(x => x.majorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.facultyId, x.majorId }).IsUnique();
        builder.HasIndex(x => x.majorId);
    }
}
