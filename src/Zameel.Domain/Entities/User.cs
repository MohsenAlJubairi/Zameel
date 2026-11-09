using Zameel.Domain.Common;

namespace Zameel.Domain.Entities;

public class User : BaseEntity
{
    public string email { get; set; } = string.Empty;
    public string? phone { get; set; }
    public string fullName { get; set; } = string.Empty;
    public string? avatarUrl { get; set; }

    // Dynamic RBAC (One Role per User)
    public Guid roleId { get; set; }
    public Role role { get; set; } = null!;

    // Dynamic Plan / Subscription (الباقات)
    public Guid? planId { get; set; }
    public Plan? plan { get; set; }
    public DateTime? planExpiresAt { get; set; }

    public Guid? universityId { get; set; }
    public University? university { get; set; }

    public Guid? facultyId { get; set; }
    public Faculty? faculty { get; set; }

    public Guid? majorId { get; set; }
    public Major? major { get; set; }

    // Academic Registration Number (رقم القيد الجامعي) - For student verification
    public string? studentRegNo { get; set; }
    public bool isVerified { get; set; } = false;
    public DateTime? verifiedAt { get; set; }

    // Quota tracking
    public int activeListingsCount { get; set; } = 0;

    // Navigation Properties
    public StoreProfile? storeProfile { get; set; }
    public ICollection<Listing> listings { get; set; } = new List<Listing>();
    public ICollection<Comment> comments { get; set; } = new List<Comment>();
    public ICollection<Review> reviewsGiven { get; set; } = new List<Review>();
    public ICollection<Review> reviewsReceived { get; set; } = new List<Review>();
    public ICollection<ListingReport> reportsSubmitted { get; set; } = new List<ListingReport>();
}
