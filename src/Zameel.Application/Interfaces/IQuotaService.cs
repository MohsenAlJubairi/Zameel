using Zameel.Application.Common;

namespace Zameel.Application.Interfaces;

public interface IQuotaService
{
    Task<ServiceResult<bool>> CanCreateListingAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ServiceResult<int>> GetRemainingQuotaAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> SyncUserActiveListingsCountAsync(Guid userId, CancellationToken cancellationToken = default);
}
