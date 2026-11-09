using Zameel.Domain.Enums;

namespace Zameel.Application.DTOs;

public record AdminDashboardStatsDto(
    int totalActiveListings,
    int pendingVerificationsCount,
    int pendingSoldReportsCount,
    int totalVerifiedStudents,
    int totalDealsConfirmed,
    int totalUsers,
    List<PendingVerificationDto> recentVerifications,
    List<SoldReportItemDto> recentSoldReports
);

public record PendingVerificationDto(
    Guid userId,
    string fullName,
    string email,
    string? phone,
    string? universityName,
    string? facultyName,
    string? majorName,
    string? studentRegNo,
    DateTime createdAt
);

public record SoldReportItemDto(
    Guid listingId,
    string title,
    decimal price,
    string sellerName,
    string sellerEmail,
    int reportedSoldCount,
    DateTime createdAt,
    List<string> reportersNames
);

public record AdminListingItemDto(
    Guid id,
    string title,
    decimal price,
    ListingType type,
    ListingStatus status,
    string sellerName,
    string universityName,
    string? facultyName,
    int viewsCount,
    int reportedSoldCount,
    DateTime createdAt
);

public record AdminUserItemDto(
    Guid id,
    string fullName,
    string email,
    string? phone,
    string roleName,
    string roleNormalized,
    bool isVerified,
    string? universityName,
    int activeListingsCount,
    DateTime createdAt,
    string? planName = null
);

public record AdminStoreItemDto(
    Guid id,
    Guid userId,
    string storeName,
    string ownerName,
    string ownerEmail,
    string? commercialRegister,
    bool isApproved,
    DateTime subscriptionExpiresAt,
    DateTime createdAt
);

// Academic Catalog Management DTOs
public record AdminUniversityItemDto(
    Guid id,
    string name,
    string city,
    string? logoUrl,
    bool isActive,
    int facultiesCount,
    int studentsCount,
    int listingsCount,
    DateTime createdAt
);

public record AdminUniversityDetailDto(
    Guid id,
    string name,
    string city,
    string? logoUrl,
    bool isActive,
    int studentsCount,
    int listingsCount,
    List<AdminUniversityFacultyDto> faculties,
    List<FacultyDto> availableFacultiesToLink
);

public record AdminUniversityFacultyDto(
    Guid universityFacultyId,
    Guid facultyId,
    string facultyName,
    string? customName,
    string slug,
    bool isActive,
    int majorsCount,
    int listingsCount
);

public record AdminFacultyItemDto(
    Guid id,
    string name,
    string slug,
    bool isActive,
    int universitiesCount,
    int majorsCount,
    DateTime createdAt
);

public record AdminFacultyDetailDto(
    Guid id,
    string name,
    string slug,
    bool isActive,
    int universitiesCount,
    List<AdminMajorItemDto> majors,
    List<MajorDto> availableMajorsToLink
);

public record AdminMajorItemDto(
    Guid id,
    string name,
    string slug,
    bool isActive
);

public record CreateUniversityRequestDto(
    string name,
    string city,
    string? logoUrl
);

public record UpdateUniversityRequestDto(
    Guid id,
    string name,
    string city,
    string? logoUrl,
    bool isActive
);

public record CreateFacultyRequestDto(
    string name,
    string slug
);

public record UpdateFacultyRequestDto(
    Guid id,
    string name,
    string slug,
    bool isActive
);

public record LinkFacultyToUniversityRequestDto(
    Guid universityId,
    Guid facultyId,
    string? customName
);

public record CreateMajorRequestDto(
    string name,
    string slug
);

public record LinkMajorToFacultyRequestDto(
    Guid facultyId,
    Guid majorId
);
