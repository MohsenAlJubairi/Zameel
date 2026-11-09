using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;
using Zameel.Domain.Constants;
using Zameel.Web.Models;

namespace Zameel.Web.Controllers;

[Route("auth")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IAcademicCatalogService _catalogService;
    private readonly IPermissionService _permissionService;

    public AuthController(
        IAuthService authService,
        IAcademicCatalogService catalogService,
        IPermissionService permissionService)
    {
        _authService = authService;
        _catalogService = catalogService;
        _permissionService = permissionService;
    }

    [HttpGet("login")]
    public IActionResult Login(string? returnUrl)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole("Admin")) return Redirect("/Admin");
            if (User.IsInRole("Store")) return Redirect("/store");
            return LocalRedirect(GetSafeReturnUrl(returnUrl));
        }

        var model = new LoginViewModel
        {
            returnUrl = returnUrl
        };

        return View(model);
    }

    [HttpPost("send-otp")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendOtp(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.emailOrPhone))
        {
            model.errorMessage = "يرجى إدخال البريد الإلكتروني.";
            return View("Login", model);
        }

        var result = await _authService.SendOtpAsync(model.emailOrPhone.Trim(), cancellationToken);
        if (!result.isSuccess)
        {
            model.errorMessage = result.message;
            return View("Login", model);
        }

        model.otpSent = true;
        model.infoMessage = "تم إرسال رمز التحقق (رقم PIN) إلى بريدك الإلكتروني بنجاح.";
        return View("Login", model);
    }

    [HttpPost("verify-otp")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyOtp(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.emailOrPhone) || string.IsNullOrWhiteSpace(model.otpCode))
        {
            model.otpSent = true;
            model.errorMessage = "يرجى إدخال رمز التحقق (رقم PIN).";
            return View("Login", model);
        }

        var result = await _authService.VerifyOtpAsync(model.emailOrPhone.Trim(), model.otpCode.Trim(), cancellationToken);
        if (!result.isSuccess)
        {
            // If account is not registered yet, redirect to registration
            if (result.message.Contains("غير مسجل"))
            {
                return RedirectToAction(nameof(Register), new { email = model.emailOrPhone, returnUrl = model.returnUrl });
            }

            model.otpSent = true;
            model.errorMessage = result.message;
            return View("Login", model);
        }

        await SignInUserAsync(result.data!);

        return LocalRedirect(GetSafeReturnUrl(model.returnUrl, result.data!.role));
    }

    [HttpGet("register")]
    public IActionResult Register(string? email, string? returnUrl)
    {
        var model = new RegisterViewModel
        {
            email = email ?? string.Empty,
            returnUrl = returnUrl
        };

        return View(model);
    }

    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.fullName) || string.IsNullOrWhiteSpace(model.email) || string.IsNullOrWhiteSpace(model.phone))
        {
            model.errorMessage = "يرجى تعبئة كافة الحقول (الاسم الكامل، البريد الإلكتروني، ورقم الهاتف).";
            return View(model);
        }

        var dto = new RegisterStudentDto(
            model.fullName,
            model.email,
            model.phone
        );

        var result = await _authService.RegisterStudentAsync(dto, cancellationToken);
        if (!result.isSuccess || result.data == null)
        {
            model.errorMessage = result.message;
            return View(model);
        }

        await SignInUserAsync(result.data);
        TempData["SuccessMessage"] = $"أهلاً بك يا {result.data.fullName} في زميل! تم إنشاء حسابك بنجاح.";

        return LocalRedirect(GetSafeReturnUrl(model.returnUrl, result.data.roleNormalized));
    }


    [HttpGet("logout")]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInUserAsync(AuthUserDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.id.ToString()),
            new(ClaimTypes.Name, user.fullName),
            new(ClaimTypes.Email, user.email),
            new(ClaimTypes.Role, user.roleNormalized),
            new("RoleName", user.role),
            new("RoleNormalized", user.roleNormalized),
            new("IsVerified", user.isVerified.ToString()),
            new("ActiveListingsCount", user.activeListingsCount.ToString())
        };

        // Add standard friendly role aliases for compatibility
        if (user.roleNormalized == SystemRoles.Admin)
            claims.Add(new(ClaimTypes.Role, "Admin"));
        else if (user.roleNormalized == SystemRoles.Store)
            claims.Add(new(ClaimTypes.Role, "Store"));
        else if (user.roleNormalized == SystemRoles.Moderator)
            claims.Add(new(ClaimTypes.Role, "Moderator"));

        if (!string.IsNullOrEmpty(user.planName))
            claims.Add(new("PlanName", user.planName));

        if (user.universityId.HasValue)
            claims.Add(new("UniversityId", user.universityId.Value.ToString()));

        if (user.facultyId.HasValue)
            claims.Add(new("FacultyId", user.facultyId.Value.ToString()));

        if (!string.IsNullOrEmpty(user.universityName))
            claims.Add(new("UniversityName", user.universityName));

        // Dynamically append all granted permissions as individual claims
        var permissions = await _permissionService.GetUserPermissionsAsync(user.id);
        foreach (var permission in permissions)
        {
            claims.Add(new("Permission", permission));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
    }

    private static string GetRoleDefaultUrl(string? roleNormalized)
    {
        if (string.Equals(roleNormalized, SystemRoles.Admin, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(roleNormalized, "Admin", StringComparison.OrdinalIgnoreCase))
            return "/Admin";

        if (string.Equals(roleNormalized, SystemRoles.Store, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(roleNormalized, "Store", StringComparison.OrdinalIgnoreCase))
            return "/store";

        if (string.Equals(roleNormalized, SystemRoles.Moderator, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(roleNormalized, "Moderator", StringComparison.OrdinalIgnoreCase))
            return "/Admin";

        return "/";
    }

    private static string GetSafeReturnUrl(string? returnUrl, string? roleNormalized = null)
    {
        if (string.IsNullOrEmpty(returnUrl) || returnUrl == "/" || !returnUrl.StartsWith('/'))
        {
            return !string.IsNullOrEmpty(roleNormalized) ? GetRoleDefaultUrl(roleNormalized) : "/";
        }
        return returnUrl;
    }
}
