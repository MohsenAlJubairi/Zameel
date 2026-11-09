using Zameel.Application.Common;
using Zameel.Application.DTOs;

namespace Zameel.Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<string>> SendOtpAsync(string emailOrPhone, CancellationToken cancellationToken = default);

    Task<ServiceResult<AuthUserDto>> VerifyOtpAsync(string emailOrPhone, string otpCode, CancellationToken cancellationToken = default);

    Task<ServiceResult<AuthUserDto>> RegisterStudentAsync(RegisterStudentDto dto, CancellationToken cancellationToken = default);

    Task<ServiceResult<AuthUserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<ServiceResult<List<DemoUserItemDto>>> GetDemoUsersAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<AuthUserDto>> LoginAsDemoUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
