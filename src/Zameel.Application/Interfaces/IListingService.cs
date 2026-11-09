using Zameel.Application.Common;
using Zameel.Application.DTOs;

namespace Zameel.Application.Interfaces;

public interface IListingService
{
    Task<ServiceResult<PagedResult<ListingCardDto>>> GetFeedAsync(ListingFilterDto filter, CancellationToken cancellationToken = default);
    Task<ServiceResult<ListingDetailsDto>> GetByIdAsync(Guid id, Guid? currentUserId = null, CancellationToken cancellationToken = default);
    Task<ServiceResult<Guid>> CreateListingAsync(CreateListingDto dto, List<string> imageUrls, Guid userId, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> MarkAsSoldAsync(Guid listingId, Guid userId, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> RenewListingAsync(Guid listingId, Guid userId, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> ReportSoldAsync(Guid listingId, Guid reporterId, CancellationToken cancellationToken = default);
}
