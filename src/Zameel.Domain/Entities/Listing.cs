using Zameel.Domain.Common;
using Zameel.Domain.Enums;

namespace Zameel.Domain.Entities;

public class Listing : BaseEntity
{
    public Guid userId { get; set; }
    public User user { get; set; } = null!;

    public Guid universityId { get; set; }
    public University university { get; set; } = null!;

    public Guid? primaryFacultyId { get; set; }
    public Faculty? primaryFaculty { get; set; }

    public Guid? primaryMajorId { get; set; }
    public Major? primaryMajor { get; set; }

    public string title { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public decimal price { get; set; } = 0m;

    public ListingType type { get; set; } = ListingType.ForSale;
    public ItemCondition condition { get; set; } = ItemCondition.Used;
    public ListingScope scope { get; set; } = ListingScope.MajorSpecific;
    public ListingStatus status { get; set; } = ListingStatus.Active;

    public int reportedSoldCount { get; set; } = 0;
    public int viewsCount { get; set; } = 0;

    public DateTime expiresAt { get; set; }
    public DateTime? soldAt { get; set; }

    // Pickup / Drop-off Campus Bookstore Hub
    public Guid? pickupStoreId { get; set; }
    public StoreProfile? pickupStore { get; set; }

    // Navigation Properties
    public ICollection<ListingImage> images { get; set; } = new List<ListingImage>();
    public ICollection<ListingFaculty> targetFaculties { get; set; } = new List<ListingFaculty>();
    public ICollection<ListingMajor> targetMajors { get; set; } = new List<ListingMajor>();
    public ICollection<Comment> comments { get; set; } = new List<Comment>();
    public ICollection<ListingReport> reports { get; set; } = new List<ListingReport>();
    public ICollection<Review> reviews { get; set; } = new List<Review>();
}
