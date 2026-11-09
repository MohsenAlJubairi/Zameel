using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Zameel.Application.Common;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;
using Zameel.Domain.Constants;
using Zameel.Domain.Entities;
using Zameel.Domain.Enums;

namespace Zameel.Application.Services;

public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _context;

    // Thread-safe in-memory OTP cache for KISS OTP handling
    private static readonly ConcurrentDictionary<string, (string Code, DateTime Expiry)> _otpCache = new();

    public AuthService(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<ServiceResult<string>> SendOtpAsync(string emailOrPhone, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(emailOrPhone))
            return Task.FromResult(ServiceResult<string>.Fail("يرجى إدخال البريد الإلكتروني."));

        var key = emailOrPhone.Trim().ToLowerInvariant();
        // Generate a random 4-digit PIN code
        var otp = Random.Shared.Next(1000, 9999).ToString();
        var expiry = DateTime.UtcNow.AddMinutes(10);

        _otpCache[key] = (otp, expiry);

        // Server log for production monitoring / terminal verification
        Console.WriteLine($"[Zameel-Auth] Verification PIN for {key}: {otp}");

        return Task.FromResult(ServiceResult<string>.Success(otp, $"تم إرسال رمز التحقق (رقم PIN) بنجاح إلى: {emailOrPhone}"));
    }

    public async Task<ServiceResult<AuthUserDto>> VerifyOtpAsync(string emailOrPhone, string otpCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(emailOrPhone) || string.IsNullOrWhiteSpace(otpCode))
            return ServiceResult<AuthUserDto>.Fail("يرجى إدخال البريد الإلكتروني ورقم البن (PIN).");

        var key = emailOrPhone.Trim().ToLowerInvariant();
        var code = otpCode.Trim();

        bool isValid = false;
        if (_otpCache.TryGetValue(key, out var cached))
        {
            if (cached.Code == code && cached.Expiry > DateTime.UtcNow)
            {
                isValid = true;
                _otpCache.TryRemove(key, out _);
            }
        }

        if (!isValid)
            return ServiceResult<AuthUserDto>.Fail("رمز التحقق (رقم PIN) غير صحيح أو انتهت صلاحيته. يرجى طلب رمز جديد.");

        // Check if user exists
        var user = await _context.users
            .Include(u => u.university)
            .Include(u => u.faculty)
            .Include(u => u.role)
            .Include(u => u.plan)
            .FirstOrDefaultAsync(u => u.email.ToLower() == key || (u.phone != null && u.phone == key), cancellationToken);

        if (user == null)
        {
            // Not registered yet
            return ServiceResult<AuthUserDto>.Fail("الحساب غير مسجل، يرجى استكمال بيانات التسجيل أولاً.");
        }

        return ServiceResult<AuthUserDto>.Success(MapToAuthDto(user), "تم تسجيل الدخول بنجاح.");
    }

    public async Task<ServiceResult<AuthUserDto>> RegisterStudentAsync(RegisterStudentDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.fullName) || string.IsNullOrWhiteSpace(dto.email))
            return ServiceResult<AuthUserDto>.Fail("يرجى إدخال الاسم الكامل والبريد الإلكتروني.");

        var email = dto.email.Trim().ToLowerInvariant();
        var existing = await _context.users.AnyAsync(u => u.email.ToLower() == email, cancellationToken);
        if (existing)
            return ServiceResult<AuthUserDto>.Fail("هذا البريد الإلكتروني مسجل مسبقاً، يمكنك تسجيل الدخول مباشرة.");

        var defaultRole = await _context.roles
            .FirstOrDefaultAsync(r => r.normalizedName == SystemRoles.StudentUnverified, cancellationToken)
            ?? await _context.roles.FirstOrDefaultAsync(cancellationToken);

        if (defaultRole == null)
            return ServiceResult<AuthUserDto>.Fail("خطأ في تهيئة النظام: لا يوجد أدوار محددة.");

        var defaultPlan = await _context.plans
            .FirstOrDefaultAsync(p => p.isDefault && p.isActive, cancellationToken);

        var user = new User
        {
            fullName = dto.fullName.Trim(),
            email = email,
            phone = string.IsNullOrWhiteSpace(dto.phone) ? null : dto.phone.Trim(),
            universityId = dto.universityId,
            facultyId = dto.facultyId,
            roleId = defaultRole.id,
            planId = defaultPlan?.id,
            isVerified = false,
            activeListingsCount = 0
        };

        await _context.users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Fetch with relations
        var createdUser = await _context.users
            .Include(u => u.university)
            .Include(u => u.faculty)
            .Include(u => u.role)
            .Include(u => u.plan)
            .FirstAsync(u => u.id == user.id, cancellationToken);

        return ServiceResult<AuthUserDto>.Success(MapToAuthDto(createdUser), "تم إنشاء الحساب بنجاح، مرحباً بك في زميل!");
    }

    public async Task<ServiceResult<AuthUserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.users
            .AsNoTracking()
            .Include(u => u.university)
            .Include(u => u.faculty)
            .Include(u => u.role)
            .Include(u => u.plan)
            .FirstOrDefaultAsync(u => u.id == userId, cancellationToken);

        if (user == null)
            return ServiceResult<AuthUserDto>.Fail(AppMessages.ItemNotFound);

        return ServiceResult<AuthUserDto>.Success(MapToAuthDto(user));
    }

    public async Task<ServiceResult<List<DemoUserItemDto>>> GetDemoUsersAsync(CancellationToken cancellationToken = default)
    {
        var officialEmails = new[]
        {
            "admin@zameel.app",
            "aden.bookstore@zameel.app",
            "ahmed.student@zameel.app",
            "sara.student@zameel.app"
        };

        var users = await _context.users
            .AsNoTracking()
            .Include(u => u.role)
            .Where(u => officialEmails.Contains(u.email))
            .OrderBy(u => u.createdAt)
            .Select(u => new DemoUserItemDto(
                u.id,
                u.fullName,
                u.email,
                u.role.normalizedName,
                u.isVerified,
                u.role.name
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<DemoUserItemDto>>.Success(users);
    }

    public async Task<ServiceResult<AuthUserDto>> LoginAsDemoUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.users
            .Include(u => u.university)
            .Include(u => u.faculty)
            .Include(u => u.role)
            .Include(u => u.plan)
            .FirstOrDefaultAsync(u => u.id == userId, cancellationToken);

        if (user == null)
            return ServiceResult<AuthUserDto>.Fail("المستخدم غير موجود.");

        return ServiceResult<AuthUserDto>.Success(MapToAuthDto(user), $"تم تسجيل الدخول بحساب: {user.fullName}");
    }

    private static AuthUserDto MapToAuthDto(User user)
    {
        return new AuthUserDto(
            user.id,
            user.fullName,
            user.email,
            user.phone,
            user.role != null ? user.role.name : "مستخدم",
            user.role != null ? user.role.normalizedName : SystemRoles.StudentUnverified,
            user.isVerified,
            user.universityId,
            user.university != null ? user.university.name : null,
            user.facultyId,
            user.faculty != null ? user.faculty.name : null,
            user.activeListingsCount,
            user.plan != null ? user.plan.name : null
        );
    }
}
