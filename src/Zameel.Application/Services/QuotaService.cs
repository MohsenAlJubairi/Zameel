using Microsoft.EntityFrameworkCore;
using Zameel.Application.Common;
using Zameel.Application.Interfaces;
using Zameel.Domain.Constants;
using Zameel.Domain.Enums;

namespace Zameel.Application.Services;

public class QuotaService : IQuotaService
{
    private readonly IApplicationDbContext _context;

    public QuotaService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<bool>> CanCreateListingAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            return ServiceResult<bool>.Fail(AppMessages.UnauthorizedAccess);
        }

        var user = await _context.users
            .AsNoTracking()
            .Where(u => u.id == userId)
            .Select(u => new
            {
                RoleNormalized = u.role.normalizedName,
                u.isVerified,
                u.activeListingsCount,
                PlanMaxListings = u.plan != null ? (int?)u.plan.maxActiveListings : null,
                PlanExpiresAt = u.planExpiresAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);
        }

        // Stores and Admins have unlimited or dedicated quotas
        if (user.RoleNormalized == SystemRoles.Store || user.RoleNormalized == SystemRoles.Admin)
        {
            return ServiceResult<bool>.Success(true);
        }

        int maxAllowed;
        if (user.PlanMaxListings.HasValue && (!user.PlanExpiresAt.HasValue || user.PlanExpiresAt.Value > DateTime.UtcNow))
        {
            maxAllowed = user.PlanMaxListings.Value;
        }
        else
        {
            maxAllowed = user.isVerified
                ? SystemConstants.VerifiedStudentMaxActiveListings
                : SystemConstants.UnverifiedStudentMaxActiveListings;
        }

        if (user.activeListingsCount >= maxAllowed)
        {
            string message = user.isVerified
                ? AppMessages.QuotaExceededVerified
                : AppMessages.QuotaExceededUnverified;

            return ServiceResult<bool>.Fail(message);
        }

        return ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<int>> GetRemainingQuotaAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            return ServiceResult<int>.Fail(AppMessages.UnauthorizedAccess);
        }

        var user = await _context.users
            .AsNoTracking()
            .Where(u => u.id == userId)
            .Select(u => new
            {
                RoleNormalized = u.role.normalizedName,
                u.isVerified,
                u.activeListingsCount,
                PlanMaxListings = u.plan != null ? (int?)u.plan.maxActiveListings : null,
                PlanExpiresAt = u.planExpiresAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            return ServiceResult<int>.Fail(AppMessages.ItemNotFound);
        }

        if (user.RoleNormalized == SystemRoles.Store || user.RoleNormalized == SystemRoles.Admin)
        {
            return ServiceResult<int>.Success(999);
        }

        int maxAllowed;
        if (user.PlanMaxListings.HasValue && (!user.PlanExpiresAt.HasValue || user.PlanExpiresAt.Value > DateTime.UtcNow))
        {
            maxAllowed = user.PlanMaxListings.Value;
        }
        else
        {
            maxAllowed = user.isVerified
                ? SystemConstants.VerifiedStudentMaxActiveListings
                : SystemConstants.UnverifiedStudentMaxActiveListings;
        }

        int remaining = Math.Max(0, maxAllowed - user.activeListingsCount);
        return ServiceResult<int>.Success(remaining);
    }

    public async Task<ServiceResult<bool>> SyncUserActiveListingsCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            return ServiceResult<bool>.Fail(AppMessages.UnauthorizedAccess);
        }

        var user = await _context.users
            .FirstOrDefaultAsync(u => u.id == userId, cancellationToken);

        if (user == null)
        {
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);
        }

        int actualActiveCount = await _context.listings
            .CountAsync(l => l.userId == userId && l.status == ListingStatus.Active, cancellationToken);

        user.activeListingsCount = actualActiveCount;
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true);
    }
}
