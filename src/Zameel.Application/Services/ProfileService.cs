using Microsoft.EntityFrameworkCore;
using Zameel.Application.Common;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;
using Zameel.Domain.Constants;
using Zameel.Domain.Enums;

namespace Zameel.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IApplicationDbContext _context;

    public ProfileService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<UserProfileDto>> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            return ServiceResult<UserProfileDto>.Fail(AppMessages.UnauthorizedAccess);
        }

        var user = await _context.users
            .AsNoTracking()
            .Where(u => u.id == userId)
            .Select(u => new
            {
                u.id,
                u.fullName,
                u.email,
                u.phone,
                u.avatarUrl,
                RoleName = u.role.name,
                RoleNormalized = u.role.normalizedName,
                u.isVerified,
                u.verifiedAt,
                u.studentRegNo,
                u.activeListingsCount,
                PlanName = u.plan != null ? u.plan.name : null,
                PlanMaxListings = u.plan != null ? (int?)u.plan.maxActiveListings : null,
                PlanExpiresAt = u.planExpiresAt,
                UniversityName = u.university != null ? u.university.name : null,
                FacultyName = u.faculty != null ? u.faculty.name : null,
                MajorName = u.major != null ? u.major.name : null
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            return ServiceResult<UserProfileDto>.Fail(AppMessages.ItemNotFound);
        }

        int maxAllowed;
        if (user.RoleNormalized == SystemRoles.Admin || user.RoleNormalized == SystemRoles.Store)
        {
            maxAllowed = 999;
        }
        else if (user.PlanMaxListings.HasValue && (!user.PlanExpiresAt.HasValue || user.PlanExpiresAt.Value > DateTime.UtcNow))
        {
            maxAllowed = user.PlanMaxListings.Value;
        }
        else
        {
            maxAllowed = user.isVerified ? SystemConstants.VerifiedStudentMaxActiveListings : SystemConstants.UnverifiedStudentMaxActiveListings;
        }

        int remainingQuota = Math.Max(0, maxAllowed - user.activeListingsCount);

        // Calculate ratings
        var reviewsQuery = _context.reviews.AsNoTracking().Where(r => r.revieweeId == userId);
        var totalReviews = await reviewsQuery.CountAsync(cancellationToken);
        double avgRating = totalReviews > 0 ? await reviewsQuery.AverageAsync(r => r.rating, cancellationToken) : 0.0;

        // Retrieve user's listings
        var myListings = await _context.listings
            .AsNoTracking()
            .Where(l => l.userId == userId)
            .OrderByDescending(l => l.createdAt)
            .Select(l => new ListingCardDto(
                l.id,
                l.title,
                l.price,
                l.type,
                l.condition,
                l.scope,
                l.images.Where(i => i.isPrimary).Select(i => i.imageUrl).FirstOrDefault() ??
                l.images.Select(i => i.imageUrl).FirstOrDefault(),
                l.university.name,
                l.primaryFaculty != null ? l.primaryFaculty.name : null,
                l.primaryMajor != null ? l.primaryMajor.name : null,
                user.fullName,
                user.isVerified,
                l.viewsCount,
                l.createdAt,
                l.pickupStore != null ? l.pickupStore.storeName : null,
                l.pickupStoreId,
                l.status
            ))
            .ToListAsync(cancellationToken);

        var dto = new UserProfileDto(
            user.id,
            user.fullName,
            user.email,
            user.phone,
            user.avatarUrl,
            user.RoleName,
            user.RoleNormalized,
            user.isVerified,
            user.verifiedAt,
            user.studentRegNo,
            user.UniversityName,
            user.FacultyName,
            user.MajorName,
            user.activeListingsCount,
            maxAllowed,
            remainingQuota,
            Math.Round(avgRating, 1),
            totalReviews,
            myListings,
            user.PlanName
        );

        return ServiceResult<UserProfileDto>.Success(dto);
    }

    public async Task<ServiceResult<bool>> SubmitVerificationAsync(SubmitVerificationDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.userId == Guid.Empty || dto.universityId == Guid.Empty || dto.facultyId == Guid.Empty || string.IsNullOrWhiteSpace(dto.studentRegNo))
        {
            return ServiceResult<bool>.Fail("يرجى اختيار الجامعة والكلية وإدخال رقم القيد الجامعي.");
        }

        var cleanRegNo = dto.studentRegNo.Trim();

        // Anti-multi-account check: Has this student registration number already been used for this university?
        var duplicateExists = await _context.users
            .AnyAsync(u => u.id != dto.userId && u.universityId == dto.universityId && u.studentRegNo == cleanRegNo, cancellationToken);

        if (duplicateExists)
        {
            return ServiceResult<bool>.Fail(AppMessages.DuplicateStudentRegNo);
        }

        var user = await _context.users
            .FirstOrDefaultAsync(u => u.id == dto.userId, cancellationToken);

        if (user == null)
        {
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);
        }

        user.universityId = dto.universityId;
        user.facultyId = dto.facultyId;
        user.majorId = dto.majorId;
        user.studentRegNo = cleanRegNo;

        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true, AppMessages.VerificationSubmitted);
    }
}
