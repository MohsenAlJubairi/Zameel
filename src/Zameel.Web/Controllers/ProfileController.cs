using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Zameel.Application.Common;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;

namespace Zameel.Web.Controllers;

[Route("profile")]
public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly IReviewService _reviewService;
    private readonly IAcademicCatalogService _catalogService;

    public ProfileController(
        IProfileService profileService,
        IReviewService reviewService,
        IAcademicCatalogService catalogService)
    {
        _profileService = profileService;
        _reviewService = reviewService;
        _catalogService = catalogService;
    }

    private Guid? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out var id) ? id : null;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            return RedirectToAction("Login", "Auth", new { returnUrl = "/profile" });
        }

        var result = await _profileService.GetProfileAsync(currentUserId.Value, cancellationToken);
        if (!result.isSuccess || result.data == null)
        {
            return NotFound();
        }

        var unis = await _catalogService.GetActiveUniversitiesAsync(cancellationToken);
        ViewBag.Universities = unis.data ?? new();

        return View(result.data);
    }

    [HttpPost("submit-verification")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitVerification(Guid universityId, Guid facultyId, Guid? majorId, string studentRegNo, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            return RedirectToAction("Login", "Auth", new { returnUrl = "/profile" });
        }

        if (universityId == Guid.Empty || facultyId == Guid.Empty || string.IsNullOrWhiteSpace(studentRegNo))
        {
            TempData["ErrorMessage"] = "يرجى اختيار الجامعة والكلية وإدخال رقم القيد الجامعي بشكل صحيح.";
            return RedirectToAction(nameof(Index));
        }

        var dto = new SubmitVerificationDto(currentUserId.Value, universityId, facultyId, majorId, studentRegNo);
        var result = await _profileService.SubmitVerificationAsync(dto, cancellationToken);
        if (result.isSuccess)
        {
            TempData["SuccessMessage"] = result.message;
        }
        else
        {
            TempData["ErrorMessage"] = result.message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("reviews")]
    public async Task<IActionResult> Reviews(CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            return RedirectToAction("Login", "Auth", new { returnUrl = "/profile/reviews" });
        }

        var result = await _reviewService.GetUserReviewsAsync(currentUserId.Value, cancellationToken);
        return View(result.data ?? new List<ReviewItemDto>());
    }

    [HttpPost("reviews/submit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitReview(Guid listingId, Guid revieweeId, int rating, string? comment, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            return RedirectToAction("Login", "Auth");
        }

        var dto = new SubmitReviewDto(listingId, currentUserId.Value, revieweeId, rating, comment);
        var result = await _reviewService.SubmitReviewAsync(dto, cancellationToken);
        if (result.isSuccess)
        {
            TempData["SuccessMessage"] = result.message;
        }
        else
        {
            TempData["ErrorMessage"] = result.message;
        }

        return RedirectToAction("Details", "Listings", new { id = listingId });
    }
}
