using Zameel.Domain.Common;
using Zameel.Domain.Enums;

namespace Zameel.Domain.Entities;

public class ListingReport : BaseEntity
{
    public Guid listingId { get; set; }
    public Listing listing { get; set; } = null!;

    public Guid reporterId { get; set; }
    public User reporter { get; set; } = null!;

    public ReportReason reason { get; set; }
    public string? notes { get; set; }
    public bool isProcessed { get; set; } = false;
}
