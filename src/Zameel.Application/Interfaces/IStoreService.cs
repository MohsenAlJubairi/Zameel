using Zameel.Application.Common;
using Zameel.Application.DTOs;

namespace Zameel.Application.Interfaces;

public interface IStoreService
{
    Task<ServiceResult<StoreDashboardDto>> GetStoreDashboardAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> UpdateStoreProfileAsync(Guid userId, UpdateStoreProfileDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> ApproveDropoffListingAsync(Guid listingId, Guid storeUserId, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> RejectDropoffListingAsync(Guid listingId, Guid storeUserId, string? reason = null, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<StoreDropdownDto>>> GetApprovedStoresByUniversityAsync(Guid universityId, CancellationToken cancellationToken = default);
}
