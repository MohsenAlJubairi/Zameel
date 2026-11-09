using Zameel.Application.DTOs;

namespace Zameel.Web.Models;

public class LoginViewModel
{
    public string? emailOrPhone { get; set; }
    public string? otpCode { get; set; }
    public string? returnUrl { get; set; }
    public bool otpSent { get; set; } = false;
    public string? infoMessage { get; set; }
    public string? errorMessage { get; set; }
    public List<DemoUserItemDto> demoUsers { get; set; } = new();
}

public class RegisterViewModel
{
    public string fullName { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string? phone { get; set; }
    public string? returnUrl { get; set; }
    public string? errorMessage { get; set; }
}
