using Zameel.Domain.Common;

namespace Zameel.Domain.Entities;

public class Comment : BaseEntity
{
    public Guid listingId { get; set; }
    public Listing listing { get; set; } = null!;

    public Guid userId { get; set; }
    public User user { get; set; } = null!;

    public Guid? parentCommentId { get; set; }
    public Comment? parentComment { get; set; }

    public string content { get; set; } = string.Empty;

    // Navigation Properties
    public ICollection<Comment> replies { get; set; } = new List<Comment>();
}
