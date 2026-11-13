using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("chatMessages");

        builder.HasKey(x => x.id);
        builder.Property(x => x.id).HasColumnName("id");
        builder.Property(x => x.senderId).HasColumnName("senderId").IsRequired();
        builder.Property(x => x.receiverId).HasColumnName("receiverId").IsRequired();
        builder.Property(x => x.listingId).HasColumnName("listingId");
        builder.Property(x => x.content).HasColumnName("content").HasMaxLength(1500).IsRequired();
        builder.Property(x => x.isRead).HasColumnName("isRead").HasDefaultValue(false);
        builder.Property(x => x.readAt).HasColumnName("readAt");
        builder.Property(x => x.createdAt).HasColumnName("createdAt").IsRequired();
        builder.Property(x => x.updatedAt).HasColumnName("updatedAt");
        builder.Property(x => x.isDeleted).HasColumnName("isDeleted").HasDefaultValue(false);

        builder.HasOne(x => x.sender)
            .WithMany(u => u.sentMessages)
            .HasForeignKey(x => x.senderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.receiver)
            .WithMany(u => u.receivedMessages)
            .HasForeignKey(x => x.receiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.listing)
            .WithMany(l => l.chatMessages)
            .HasForeignKey(x => x.listingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.senderId, x.receiverId, x.createdAt });
        builder.HasIndex(x => new { x.receiverId, x.isRead });
    }
}
