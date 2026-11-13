using Zameel.Domain.Common;

namespace Zameel.Domain.Entities;

public class ChatMessage : BaseEntity
{
    public Guid senderId { get; set; }
    public User sender { get; set; } = null!;

    public Guid receiverId { get; set; }
    public User receiver { get; set; } = null!;

    public Guid? listingId { get; set; }
    public Listing? listing { get; set; }

    public string content { get; set; } = string.Empty;
    public bool isRead { get; set; } = false;
    public DateTime? readAt { get; set; }
}
