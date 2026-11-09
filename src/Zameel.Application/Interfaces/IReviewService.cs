using Zameel.Application.Common;
using Zameel.Application.DTOs;

namespace Zameel.Application.Interfaces;

public interface IReviewService
{
    Task<ServiceResult<bool>> SubmitReviewAsync(SubmitReviewDto dto, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<ReviewItemDto>>> GetUserReviewsAsync(Guid userId, CancellationToken cancellationToken = default);
}
