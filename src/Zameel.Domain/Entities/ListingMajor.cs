using Zameel.Domain.Common;

namespace Zameel.Domain.Entities;

public class ListingMajor : BaseEntity
{
    public Guid listingId { get; set; }
    public Listing listing { get; set; } = null!;

    public Guid majorId { get; set; }
    public Major major { get; set; } = null!;
}
