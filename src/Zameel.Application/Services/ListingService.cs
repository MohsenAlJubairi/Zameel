using Microsoft.EntityFrameworkCore;
using Zameel.Application.Common;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;
using Zameel.Domain.Constants;
using Zameel.Domain.Entities;
using Zameel.Domain.Enums;

namespace Zameel.Application.Services;

public class ListingService : IListingService
{
    private readonly IApplicationDbContext _context;
    private readonly IQuotaService _quotaService;
    private readonly ICommentService _commentService;

    public ListingService(IApplicationDbContext context, IQuotaService quotaService, ICommentService commentService)
    {
        _context = context;
        _quotaService = quotaService;
        _commentService = commentService;
    }

    public async Task<ServiceResult<PagedResult<ListingCardDto>>> GetFeedAsync(ListingFilterDto filter, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, filter.page);
        var pageSize = Math.Clamp(filter.pageSize, 1, SystemConstants.MaxPageSize);

        var query = _context.listings
            .AsNoTracking()
            .Where(l => l.status == ListingStatus.Active && l.expiresAt > DateTime.UtcNow);

        // Filter by University
        if (filter.universityId.HasValue && filter.universityId.Value != Guid.Empty)
        {
            query = query.Where(l => l.universityId == filter.universityId.Value);
        }

        // Filter by Faculty (Primary or Target)
        if (filter.facultyId.HasValue && filter.facultyId.Value != Guid.Empty)
        {
            query = query.Where(l => l.primaryFacultyId == filter.facultyId.Value ||
                                     l.targetFaculties.Any(tf => tf.facultyId == filter.facultyId.Value));
        }

        // Filter by Major (Primary or Target)
        if (filter.majorId.HasValue && filter.majorId.Value != Guid.Empty)
        {
            query = query.Where(l => l.primaryMajorId == filter.majorId.Value ||
                                     l.targetMajors.Any(tm => tm.majorId == filter.majorId.Value));
        }

        // Filter by Type
        if (filter.type.HasValue)
        {
            query = query.Where(l => l.type == filter.type.Value);
        }

        // Filter by Condition
        if (filter.condition.HasValue)
        {
            query = query.Where(l => l.condition == filter.condition.Value);
        }

        // Filter by Price Range
        if (filter.minPrice.HasValue)
        {
            query = query.Where(l => l.price >= filter.minPrice.Value);
        }

        if (filter.maxPrice.HasValue)
        {
            query = query.Where(l => l.price <= filter.maxPrice.Value);
        }

        // Filter by Search Keyword
        if (!string.IsNullOrWhiteSpace(filter.searchTerm))
        {
            var term = filter.searchTerm.Trim().ToLower();
            query = query.Where(l => l.title.Contains(term) || l.description.Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(l => l.createdAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new ListingCardDto(
                l.id,
                l.title,
                l.price,
                l.type,
                l.condition,
                l.scope,
                l.images.Where(img => img.isPrimary).Select(img => img.imageUrl).FirstOrDefault() ??
                l.images.Select(img => img.imageUrl).FirstOrDefault(),
                l.university.name,
                l.primaryFaculty != null ? l.primaryFaculty.name : null,
                l.primaryMajor != null ? l.primaryMajor.name : null,
                l.user.fullName,
                l.user.isVerified,
                l.viewsCount,
                l.createdAt,
                l.pickupStore != null ? l.pickupStore.storeName : null,
                l.pickupStoreId,
                l.status
            ))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<ListingCardDto>(items, totalCount, page, pageSize);
        return ServiceResult<PagedResult<ListingCardDto>>.Success(result);
    }

    public async Task<ServiceResult<ListingDetailsDto>> GetByIdAsync(Guid id, Guid? currentUserId = null, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return ServiceResult<ListingDetailsDto>.Fail(AppMessages.ItemNotFound);
        }

        var details = await _context.listings
            .AsNoTracking()
            .Where(l => l.id == id)
            .Select(l => new ListingDetailsDto(
                l.id,
                l.title,
                l.description,
                l.price,
                l.type,
                l.condition,
                l.scope,
                l.status,
                l.universityId,
                l.university.name,
                l.primaryFacultyId,
                l.primaryFaculty != null ? l.primaryFaculty.name : null,
                l.primaryMajorId,
                l.primaryMajor != null ? l.primaryMajor.name : null,
                l.userId,
                l.user.fullName,
                l.user.avatarUrl,
                l.user.phone,
                l.user.isVerified,
                l.images.OrderBy(img => img.displayOrder).Select(img => img.imageUrl).ToList(),
                l.targetFaculties.Select(tf => tf.faculty.name).ToList(),
                l.targetMajors.Select(tm => tm.major.name).ToList(),
                l.viewsCount,
                l.createdAt,
                l.expiresAt,
                currentUserId.HasValue && l.userId == currentUserId.Value,
                l.pickupStore != null ? l.pickupStore.storeName : null,
                l.pickupStore != null ? l.pickupStore.locationDescription : null,
                l.pickupStoreId
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (details == null)
        {
            return ServiceResult<ListingDetailsDto>.Fail(AppMessages.ItemNotFound);
        }

        // Fetch Comments
        var commentsResult = await _commentService.GetListingCommentsAsync(id, currentUserId, cancellationToken);
        details = details with { comments = commentsResult.data ?? new() };

        // Increment Views Count asynchronously without waiting
        await _context.listings
            .Where(l => l.id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.viewsCount, x => x.viewsCount + 1), cancellationToken);

        return ServiceResult<ListingDetailsDto>.Success(details);
    }

    public async Task<ServiceResult<Guid>> CreateListingAsync(CreateListingDto dto, List<string> imageUrls, Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            return ServiceResult<Guid>.Fail(AppMessages.UnauthorizedAccess);
        }

        // 1. Quota Check
        var quotaCheck = await _quotaService.CanCreateListingAsync(userId, cancellationToken);
        if (!quotaCheck.isSuccess)
        {
            return ServiceResult<Guid>.Fail(quotaCheck.message);
        }

        // 2. Validate Images Count
        if (imageUrls != null && imageUrls.Count > SystemConstants.MaxImagesPerListing)
        {
            return ServiceResult<Guid>.Fail(AppMessages.ImageCountExceeded);
        }

        var user = await _context.users
            .Include(u => u.role)
            .FirstOrDefaultAsync(u => u.id == userId, cancellationToken);
        if (user == null)
        {
            return ServiceResult<Guid>.Fail(AppMessages.ItemNotFound);
        }

        ListingStatus initialStatus;
        Guid? assignedPickupStoreId;
        string resultMessage;

        if (user.role.normalizedName == SystemRoles.Store)
        {
            // Store posting its own item is directly Active
            var storeProfile = await _context.storeProfiles.FirstOrDefaultAsync(sp => sp.userId == userId, cancellationToken);
            assignedPickupStoreId = storeProfile?.id;
            initialStatus = ListingStatus.Active;
            resultMessage = AppMessages.ListingCreatedSuccess;
        }
        else
        {
            // Student must choose an approved campus bookstore as mandatory drop-off point
            if (!dto.pickupStoreId.HasValue || dto.pickupStoreId.Value == Guid.Empty)
            {
                return ServiceResult<Guid>.Fail(AppMessages.StoreRequiredForListing);
            }

            assignedPickupStoreId = dto.pickupStoreId.Value;
            initialStatus = ListingStatus.PendingConfirmation;
            resultMessage = AppMessages.ListingPendingStoreApproval;
        }

        var listing = new Listing
        {
            userId = userId,
            universityId = dto.universityId,
            primaryFacultyId = dto.primaryFacultyId,
            primaryMajorId = dto.primaryMajorId,
            title = dto.title.Trim(),
            description = dto.description.Trim(),
            price = Math.Round(Math.Max(0m, dto.price), 0, MidpointRounding.AwayFromZero),
            type = dto.type,
            condition = dto.condition,
            scope = dto.scope,
            status = initialStatus,
            pickupStoreId = assignedPickupStoreId,
            expiresAt = DateTime.UtcNow.AddDays(SystemConstants.AutoExpirationDays)
        };

        // Add Images
        if (imageUrls != null && imageUrls.Count > 0)
        {
            for (int i = 0; i < imageUrls.Count; i++)
            {
                listing.images.Add(new ListingImage
                {
                    imageUrl = imageUrls[i],
                    displayOrder = i,
                    isPrimary = (i == 0)
                });
            }
        }

        // Add Target Faculties
        if (dto.targetFacultyIds != null && dto.targetFacultyIds.Count > 0)
        {
            foreach (var facultyId in dto.targetFacultyIds)
            {
                listing.targetFaculties.Add(new ListingFaculty
                {
                    facultyId = facultyId
                });
            }
        }

        // Add Target Majors
        if (dto.targetMajorIds != null && dto.targetMajorIds.Count > 0)
        {
            foreach (var majorId in dto.targetMajorIds)
            {
                listing.targetMajors.Add(new ListingMajor
                {
                    majorId = majorId
                });
            }
        }

        await _context.listings.AddAsync(listing, cancellationToken);

        // Increment active listings counter on user
        user.activeListingsCount++;

        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<Guid>.Success(listing.id, resultMessage);
    }

    public async Task<ServiceResult<bool>> MarkAsSoldAsync(Guid listingId, Guid userId, CancellationToken cancellationToken = default)
    {
        if (listingId == Guid.Empty || userId == Guid.Empty)
        {
            return ServiceResult<bool>.Fail(AppMessages.UnauthorizedAccess);
        }

        var listing = await _context.listings
            .FirstOrDefaultAsync(l => l.id == listingId, cancellationToken);

        if (listing == null)
        {
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);
        }

        if (listing.userId != userId)
        {
            return ServiceResult<bool>.Fail(AppMessages.UnauthorizedAccess);
        }

        if (listing.status == ListingStatus.Sold)
        {
            return ServiceResult<bool>.Success(true, AppMessages.ListingMarkedAsSold);
        }

        listing.status = ListingStatus.Sold;
        listing.soldAt = DateTime.UtcNow;

        // Decrement user's active counter
        var user = await _context.users.FirstOrDefaultAsync(u => u.id == userId, cancellationToken);
        if (user != null && user.activeListingsCount > 0)
        {
            user.activeListingsCount--;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(true, AppMessages.ListingMarkedAsSold);
    }

    public async Task<ServiceResult<bool>> RenewListingAsync(Guid listingId, Guid userId, CancellationToken cancellationToken = default)
    {
        if (listingId == Guid.Empty || userId == Guid.Empty)
        {
            return ServiceResult<bool>.Fail(AppMessages.UnauthorizedAccess);
        }

        var listing = await _context.listings
            .FirstOrDefaultAsync(l => l.id == listingId, cancellationToken);

        if (listing == null)
        {
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);
        }

        if (listing.userId != userId)
        {
            return ServiceResult<bool>.Fail(AppMessages.UnauthorizedAccess);
        }

        listing.expiresAt = DateTime.UtcNow.AddDays(SystemConstants.AutoExpirationDays);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true, AppMessages.ListingRenewedSuccess);
    }

    public async Task<ServiceResult<bool>> ReportSoldAsync(Guid listingId, Guid reporterId, CancellationToken cancellationToken = default)
    {
        if (listingId == Guid.Empty || reporterId == Guid.Empty)
        {
            return ServiceResult<bool>.Fail(AppMessages.UnauthorizedAccess);
        }

        var listing = await _context.listings
            .FirstOrDefaultAsync(l => l.id == listingId, cancellationToken);

        if (listing == null)
        {
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);
        }

        listing.reportedSoldCount++;

        // If reports threshold reached, mark as sold or archived automatically to keep the marketplace clean
        if (listing.reportedSoldCount >= SystemConstants.SoldReportsThresholdToHide)
        {
            listing.status = ListingStatus.Sold;
            listing.soldAt = DateTime.UtcNow;

            var seller = await _context.users.FirstOrDefaultAsync(u => u.id == listing.userId, cancellationToken);
            if (seller != null && seller.activeListingsCount > 0)
            {
                seller.activeListingsCount--;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(true, AppMessages.ListingReportedSoldSuccess);
    }
}
