using Zameel.Domain.Common;

namespace Zameel.Domain.Entities;

public class ListingFaculty : BaseEntity
{
    public Guid listingId { get; set; }
    public Listing listing { get; set; } = null!;

    public Guid facultyId { get; set; }
    public Faculty faculty { get; set; } = null!;
}
