using Zameel.Domain.Enums;

namespace Zameel.Application.DTOs;

public record ListingFilterDto(
    Guid? universityId = null,
    Guid? facultyId = null,
    Guid? majorId = null,
    ListingType? type = null,
    ItemCondition? condition = null,
    string? searchTerm = null,
    decimal? minPrice = null,
    decimal? maxPrice = null,
    int page = 1,
    int pageSize = 12
);

public record ListingCardDto(
    Guid id,
    string title,
    decimal price,
    ListingType type,
    ItemCondition condition,
    ListingScope scope,
    string? primaryImageUrl,
    string universityName,
    string? facultyName,
    string? majorName,
    string sellerName,
    bool sellerIsVerified,
    int viewsCount,
    DateTime createdAt,
    string? pickupStoreName = null,
    Guid? pickupStoreId = null,
    ListingStatus status = ListingStatus.Active
);

public record ListingDetailsDto(
    Guid id,
    string title,
    string description,
    decimal price,
    ListingType type,
    ItemCondition condition,
    ListingScope scope,
    ListingStatus status,
    Guid universityId,
    string universityName,
    Guid? primaryFacultyId,
    string? primaryFacultyName,
    Guid? primaryMajorId,
    string? primaryMajorName,
    Guid sellerId,
    string sellerName,
    string? sellerAvatarUrl,
    string? sellerPhone,
    bool sellerIsVerified,
    List<string> imageUrls,
    List<string> targetFacultyNames,
    List<string> targetMajorNames,
    int viewsCount,
    DateTime createdAt,
    DateTime expiresAt,
    bool isOwner,
    string? pickupStoreName = null,
    string? pickupStoreLocation = null,
    Guid? pickupStoreId = null,
    List<CommentDto>? comments = null
);

public record CreateListingDto(
    Guid universityId,
    Guid? primaryFacultyId,
    Guid? primaryMajorId,
    string title,
    string description,
    decimal price,
    ListingType type,
    ItemCondition condition,
    ListingScope scope,
    List<Guid>? targetFacultyIds = null,
    List<Guid>? targetMajorIds = null,
    Guid? pickupStoreId = null
);

public record PagedResult<T>(
    List<T> items,
    int totalCount,
    int page,
    int pageSize
)
{
    public int totalPages => (int)Math.Ceiling((double)totalCount / pageSize);
    public bool hasNextPage => page < totalPages;
    public bool hasPreviousPage => page > 1;
}
