using Zameel.Domain.Common;

namespace Zameel.Domain.Entities;

public class StoreProfile : BaseEntity
{
    public Guid userId { get; set; }
    public User user { get; set; } = null!;

    public string storeName { get; set; } = string.Empty;
    public string? description { get; set; }
    public string? commercialRegister { get; set; }
    public string? locationDescription { get; set; }
    public string? logoUrl { get; set; }
    public DateTime subscriptionExpiresAt { get; set; }
    public bool isApproved { get; set; } = false;

    // Student listings deposited at this bookstore
    public ICollection<Listing> hostedListings { get; set; } = new List<Listing>();
}
