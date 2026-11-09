using Zameel.Domain.Enums;

namespace Zameel.Application.DTOs;

public record SendOtpRequestDto(
    string emailOrPhone
);

public record VerifyOtpRequestDto(
    string emailOrPhone,
    string otpCode
);

public record RegisterStudentDto(
    string fullName,
    string email,
    string? phone,
    Guid? universityId = null,
    Guid? facultyId = null,
    Guid? majorId = null
);

public record AuthUserDto(
    Guid id,
    string fullName,
    string email,
    string? phone,
    string role,
    string roleNormalized,
    bool isVerified,
    Guid? universityId,
    string? universityName,
    Guid? facultyId,
    string? facultyName,
    int activeListingsCount,
    string? planName = null
);

public record DemoUserItemDto(
    Guid id,
    string fullName,
    string email,
    string role,
    bool isVerified,
    string roleDisplayName
);
