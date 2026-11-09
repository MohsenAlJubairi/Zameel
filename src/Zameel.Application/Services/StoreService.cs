using Microsoft.EntityFrameworkCore;
using Zameel.Application.Common;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;
using Zameel.Domain.Entities;
using Zameel.Domain.Enums;

namespace Zameel.Application.Services;

public class StoreService : IStoreService
{
    private readonly IApplicationDbContext _context;

    public StoreService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<StoreDashboardDto>> GetStoreDashboardAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.users
            .AsNoTracking()
            .Include(u => u.university)
            .Include(u => u.faculty)
            .FirstOrDefaultAsync(u => u.id == userId, cancellationToken);

        if (user == null)
        {
            return ServiceResult<StoreDashboardDto>.Fail("المستخدم غير موجود.");
        }

        var profile = await _context.storeProfiles
            .FirstOrDefaultAsync(sp => sp.userId == userId, cancellationToken);

        // If profile doesn't exist yet, create a default one
        if (profile == null)
        {
            var newProfile = new StoreProfile
            {
                userId = userId,
                storeName = user.fullName,
                description = "مكتبة وخدمات جامعية لتصوير الملازم والمستلزمات الدراسية.",
                locationDescription = "داخل الحرم الجامعي",
                subscriptionExpiresAt = DateTime.UtcNow.AddMonths(3),
                isApproved = true
            };

            await _context.storeProfiles.AddAsync(newProfile, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            profile = newProfile;
        }

        // 1. Listings owned directly by the bookstore itself
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
                profile.storeName,
                profile.id,
                l.status
            ))
            .ToListAsync(cancellationToken);

        // 2. Pending student drop-offs waiting for this bookstore's approval
        var pendingDropoffs = await _context.listings
            .AsNoTracking()
            .Where(l => l.pickupStoreId == profile.id && l.status == ListingStatus.PendingConfirmation)
            .OrderByDescending(l => l.createdAt)
            .Select(l => new PendingStoreDropoffDto(
                l.id,
                l.title,
                l.price,
                l.type,
                l.condition,
                l.images.Where(i => i.isPrimary).Select(i => i.imageUrl).FirstOrDefault() ??
                l.images.Select(i => i.imageUrl).FirstOrDefault(),
                l.user.fullName,
                l.user.phone,
                l.user.studentRegNo,
                l.university.name,
                l.primaryFaculty != null ? l.primaryFaculty.name : null,
                l.createdAt
            ))
            .ToListAsync(cancellationToken);

        // 3. Active student listings deposited and approved at this bookstore
        var hostedStudentListings = await _context.listings
            .AsNoTracking()
            .Where(l => l.pickupStoreId == profile.id && l.userId != userId && l.status == ListingStatus.Active)
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
                l.user.fullName,
                l.user.isVerified,
                l.viewsCount,
                l.createdAt,
                profile.storeName,
                profile.id,
                l.status
            ))
            .ToListAsync(cancellationToken);

        var totalListings = myListings.Count + hostedStudentListings.Count;
        var activeListings = myListings.Count(l => l.status == ListingStatus.Active) + hostedStudentListings.Count;
        long totalViews = myListings.Sum(l => (long)l.viewsCount) + hostedStudentListings.Sum(l => (long)l.viewsCount);

        var dto = new StoreDashboardDto(
            profile.id,
            user.id,
            profile.storeName,
            profile.description,
            profile.locationDescription,
            profile.logoUrl,
            profile.subscriptionExpiresAt,
            profile.isApproved,
            user.university?.name,
            user.faculty?.name,
            user.fullName,
            user.email,
            user.phone,
            totalListings,
            activeListings,
            totalViews,
            myListings,
            pendingDropoffs,
            hostedStudentListings
        );

        return ServiceResult<StoreDashboardDto>.Success(dto);
    }

    public async Task<ServiceResult<bool>> ApproveDropoffListingAsync(Guid listingId, Guid storeUserId, CancellationToken cancellationToken = default)
    {
        var profile = await _context.storeProfiles
            .FirstOrDefaultAsync(sp => sp.userId == storeUserId, cancellationToken);

        if (profile == null)
        {
            return ServiceResult<bool>.Fail("ملف المكتبة غير موجود.");
        }

        var listing = await _context.listings
            .FirstOrDefaultAsync(l => l.id == listingId, cancellationToken);

        if (listing == null)
        {
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);
        }

        if (listing.pickupStoreId != profile.id)
        {
            return ServiceResult<bool>.Fail("هذه السلعة غير مودعة لدى مكتبتك.");
        }

        listing.status = ListingStatus.Active;
        listing.updatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true, AppMessages.StoreListingApproved);
    }

    public async Task<ServiceResult<bool>> RejectDropoffListingAsync(Guid listingId, Guid storeUserId, string? reason = null, CancellationToken cancellationToken = default)
    {
        var profile = await _context.storeProfiles
            .FirstOrDefaultAsync(sp => sp.userId == storeUserId, cancellationToken);

        if (profile == null)
        {
            return ServiceResult<bool>.Fail("ملف المكتبة غير موجود.");
        }

        var listing = await _context.listings
            .FirstOrDefaultAsync(l => l.id == listingId, cancellationToken);

        if (listing == null)
        {
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);
        }

        if (listing.pickupStoreId != profile.id)
        {
            return ServiceResult<bool>.Fail("هذه السلعة غير مودعة لدى مكتبتك.");
        }

        listing.status = ListingStatus.Archived;
        listing.isDeleted = true;
        listing.updatedAt = DateTime.UtcNow;

        // Restore student active listing quota
        var seller = await _context.users.FirstOrDefaultAsync(u => u.id == listing.userId, cancellationToken);
        if (seller != null && seller.activeListingsCount > 0)
        {
            seller.activeListingsCount--;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true, AppMessages.StoreListingRejected);
    }

    public async Task<ServiceResult<List<StoreDropdownDto>>> GetApprovedStoresByUniversityAsync(Guid universityId, CancellationToken cancellationToken = default)
    {
        var stores = await _context.storeProfiles
            .AsNoTracking()
            .Include(sp => sp.user)
            .ThenInclude(u => u.faculty)
            .Where(sp => sp.isApproved && sp.user.universityId == universityId)
            .OrderBy(sp => sp.storeName)
            .Select(sp => new StoreDropdownDto(
                sp.id,
                sp.storeName,
                sp.locationDescription,
                sp.user.faculty != null ? sp.user.faculty.name : null
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<StoreDropdownDto>>.Success(stores);
    }

    public async Task<ServiceResult<bool>> UpdateStoreProfileAsync(Guid userId, UpdateStoreProfileDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.storeName))
        {
            return ServiceResult<bool>.Fail("يرجى إدخال اسم المكتبة.");
        }

        var profile = await _context.storeProfiles
            .FirstOrDefaultAsync(sp => sp.userId == userId, cancellationToken);

        if (profile == null)
        {
            return ServiceResult<bool>.Fail("ملف المكتبة غير موجود.");
        }

        profile.storeName = dto.storeName.Trim();
        profile.description = dto.description?.Trim();
        profile.locationDescription = dto.locationDescription?.Trim();

        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true, "تم تحديث بيانات المكتبة بنجاح.");
    }
}
