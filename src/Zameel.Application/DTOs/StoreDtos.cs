using Zameel.Domain.Enums;

namespace Zameel.Application.DTOs;

public record PendingStoreDropoffDto(
    Guid listingId,
    string title,
    decimal price,
    ListingType type,
    ItemCondition condition,
    string? primaryImageUrl,
    string sellerName,
    string? sellerPhone,
    string? sellerRegNo,
    string universityName,
    string? facultyName,
    DateTime createdAt
);

public record StoreDropdownDto(
    Guid storeProfileId,
    string storeName,
    string? locationDescription,
    string? facultyName
);

public record StoreDashboardDto(
    Guid storeProfileId,
    Guid userId,
    string storeName,
    string? description,
    string? locationDescription,
    string? logoUrl,
    DateTime subscriptionExpiresAt,
    bool isApproved,
    string? universityName,
    string? facultyName,
    string ownerName,
    string ownerEmail,
    string? ownerPhone,
    int totalListingsCount,
    int activeListingsCount,
    long totalViews,
    List<ListingCardDto> myListings,
    List<PendingStoreDropoffDto> pendingDropoffs,
    List<ListingCardDto> hostedStudentListings
);

public record UpdateStoreProfileDto(
    string storeName,
    string? description,
    string? locationDescription
);
