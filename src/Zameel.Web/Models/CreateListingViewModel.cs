using Zameel.Application.DTOs;
using Zameel.Domain.Enums;

namespace Zameel.Web.Models;

public class CreateListingViewModel
{
    public Guid universityId { get; set; }
    public Guid? primaryFacultyId { get; set; }
    public Guid? primaryMajorId { get; set; }

    public string title { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public decimal price { get; set; } = 0m;

    public ListingType type { get; set; } = ListingType.ForSale;
    public ItemCondition condition { get; set; } = ItemCondition.Used;
    public ListingScope scope { get; set; } = ListingScope.MajorSpecific;

    public List<IFormFile> images { get; set; } = new List<IFormFile>();

    public Guid? pickupStoreId { get; set; }
    public List<StoreDropdownDto> availableStores { get; set; } = new();
    public bool isStoreUser { get; set; } = false;

    public List<UniversityDto> universities { get; set; } = new List<UniversityDto>();
    public string? errorMessage { get; set; }
}
