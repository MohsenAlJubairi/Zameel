using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class UniversityFacultyConfiguration : IEntityTypeConfiguration<UniversityFaculty>
{
    public void Configure(EntityTypeBuilder<UniversityFaculty> builder)
    {
        builder.ToTable("universityFaculties");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.universityId).HasColumnName("universityId").IsRequired();
        builder.Property(x => x.facultyId).HasColumnName("facultyId").IsRequired();
        builder.Property(x => x.customName).HasColumnName("customName").HasMaxLength(150);
        builder.Property(x => x.isActive).HasColumnName("isActive").HasDefaultValue(true);
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        builder.HasOne(x => x.university)
            .WithMany(x => x.universityFaculties)
            .HasForeignKey(x => x.universityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.faculty)
            .WithMany(x => x.universityFaculties)
            .HasForeignKey(x => x.facultyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.universityId, x.facultyId }).IsUnique();
        builder.HasIndex(x => x.facultyId);
    }
}
