using Zameel.Application.DTOs;
using Zameel.Domain.Enums;

namespace Zameel.Application.DTOs;

public record UserProfileDto(
    Guid id,
    string fullName,
    string email,
    string? phone,
    string? avatarUrl,
    string roleName,
    string roleNormalized,
    bool isVerified,
    DateTime? verifiedAt,
    string? studentRegNo,
    string? universityName,
    string? facultyName,
    string? majorName,
    int activeListingsCount,
    int maxAllowedListings,
    int remainingQuota,
    double averageRating,
    int totalReviewsCount,
    List<ListingCardDto> myListings,
    string? planName = null
);

public record SubmitVerificationDto(
    Guid userId,
    Guid universityId,
    Guid facultyId,
    Guid? majorId,
    string studentRegNo
);

public record SubmitReviewDto(
    Guid listingId,
    Guid reviewerId,
    Guid revieweeId,
    int rating,
    string? comment
);

public record ReviewItemDto(
    Guid id,
    string reviewerName,
    string? reviewerAvatarUrl,
    int rating,
    string? comment,
    DateTime createdAt
);
