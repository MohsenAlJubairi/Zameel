using Zameel.Domain.Common;

namespace Zameel.Domain.Entities;

public class ListingImage : BaseEntity
{
    public Guid listingId { get; set; }
    public Listing listing { get; set; } = null!;

    public string imageUrl { get; set; } = string.Empty;
    public int displayOrder { get; set; } = 0;
    public bool isPrimary { get; set; } = false;
}
