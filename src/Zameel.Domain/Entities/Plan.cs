using Zameel.Domain.Common;

namespace Zameel.Domain.Entities;

public class Plan : BaseEntity
{
    public string name { get; set; } = string.Empty;
    public string? description { get; set; }
    public decimal price { get; set; } = 0m;
    public int maxActiveListings { get; set; } = 3;
    public int durationDays { get; set; } = 30;
    public bool hasVerifiedBadge { get; set; } = false;
    public bool hasStoreProfile { get; set; } = false;
    public bool isDefault { get; set; } = false;
    public bool isActive { get; set; } = true;

    // Navigation Properties
    public ICollection<User> users { get; set; } = new List<User>();
}
