using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("comments");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.listingId).HasColumnName("listingId").IsRequired();
        builder.Property(x => x.userId).HasColumnName("userId").IsRequired();
        builder.Property(x => x.parentCommentId).HasColumnName("parentCommentId");
        builder.Property(x => x.content).HasColumnName("content").HasMaxLength(1000).IsRequired();
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        builder.HasOne(x => x.parentComment)
            .WithMany(x => x.replies)
            .HasForeignKey(x => x.parentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.listingId, x.createdAt });
    }
}
