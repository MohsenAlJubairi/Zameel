using Zameel.Domain.Common;

namespace Zameel.Domain.Entities;

public class Review : BaseEntity
{
    public Guid listingId { get; set; }
    public Listing listing { get; set; } = null!;

    public Guid reviewerId { get; set; }
    public User reviewer { get; set; } = null!;

    public Guid revieweeId { get; set; }
    public User reviewee { get; set; } = null!;

    public int rating { get; set; } // 1 to 5
    public string? comment { get; set; }
}
