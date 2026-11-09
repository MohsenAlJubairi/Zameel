using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.email).HasColumnName("email").HasMaxLength(256).IsRequired();
        builder.Property(x => x.phone).HasColumnName("phone").HasMaxLength(30);
        builder.Property(x => x.fullName).HasColumnName("fullName").HasMaxLength(150).IsRequired();
        builder.Property(x => x.avatarUrl).HasColumnName("avatarUrl").HasMaxLength(500);
        builder.Property(x => x.roleId).HasColumnName("roleId").IsRequired();
        builder.Property(x => x.planId).HasColumnName("planId");
        builder.Property(x => x.planExpiresAt).HasColumnName("planExpiresAt");
        builder.Property(x => x.universityId).HasColumnName("universityId");
        builder.Property(x => x.facultyId).HasColumnName("facultyId");
        builder.Property(x => x.majorId).HasColumnName("majorId");
        builder.Property(x => x.studentRegNo).HasColumnName("studentRegNo").HasMaxLength(50);
        builder.Property(x => x.isVerified).HasColumnName("isVerified").HasDefaultValue(false);
        builder.Property(x => x.verifiedAt).HasColumnName("verifiedAt");
        builder.Property(x => x.activeListingsCount).HasColumnName("activeListingsCount").HasDefaultValue(0);
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        // Relationships
        builder.HasOne(x => x.role)
            .WithMany(x => x.users)
            .HasForeignKey(x => x.roleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.plan)
            .WithMany(x => x.users)
            .HasForeignKey(x => x.planId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.university)
            .WithMany(x => x.users)
            .HasForeignKey(x => x.universityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.faculty)
            .WithMany(x => x.users)
            .HasForeignKey(x => x.facultyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.major)
            .WithMany(x => x.users)
            .HasForeignKey(x => x.majorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.storeProfile)
            .WithOne(x => x.user)
            .HasForeignKey<StoreProfile>(x => x.userId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.listings)
            .WithOne(x => x.user)
            .HasForeignKey(x => x.userId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.comments)
            .WithOne(x => x.user)
            .HasForeignKey(x => x.userId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.reviewsGiven)
            .WithOne(x => x.reviewer)
            .HasForeignKey(x => x.reviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.reviewsReceived)
            .WithOne(x => x.reviewee)
            .HasForeignKey(x => x.revieweeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.reportsSubmitted)
            .WithOne(x => x.reporter)
            .HasForeignKey(x => x.reporterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.email).IsUnique();
        builder.HasIndex(x => x.phone);
        builder.HasIndex(x => x.roleId);
        builder.HasIndex(x => x.planId);
        builder.HasIndex(x => new { x.universityId, x.roleId });
        builder.HasIndex(x => new { x.universityId, x.facultyId, x.majorId });
        builder.HasIndex(x => new { x.universityId, x.studentRegNo })
            .IsUnique()
            .HasFilter("[studentRegNo] IS NOT NULL AND [universityId] IS NOT NULL");
    }
}
