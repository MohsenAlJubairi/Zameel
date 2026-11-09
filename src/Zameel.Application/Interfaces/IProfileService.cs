using Zameel.Application.Common;
using Zameel.Application.DTOs;

namespace Zameel.Application.Interfaces;

public interface IProfileService
{
    Task<ServiceResult<UserProfileDto>> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ServiceResult<bool>> SubmitVerificationAsync(SubmitVerificationDto dto, CancellationToken cancellationToken = default);
}
