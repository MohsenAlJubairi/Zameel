using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;

namespace Zameel.Web.Controllers;

[Route("store")]
[Authorize(Roles = "Store,Admin")]
public class StoreController : Controller
{
    private readonly IStoreService _storeService;

    public StoreController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    private Guid? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out var id) ? id : null;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Auth", new { returnUrl = "/store" });
        }

        var result = await _storeService.GetStoreDashboardAsync(userId.Value, cancellationToken);
        if (!result.isSuccess || result.data == null)
        {
            TempData["ErrorMessage"] = result.message;
            return RedirectToAction("Index", "Home");
        }

        return View(result.data);
    }

    [HttpPost("update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UpdateStoreProfileDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Auth", new { returnUrl = "/store" });
        }

        var result = await _storeService.UpdateStoreProfileAsync(userId.Value, dto, cancellationToken);
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

    [HttpPost("approve-dropoff")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveDropoff(Guid listingId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Auth", new { returnUrl = "/store" });
        }

        var result = await _storeService.ApproveDropoffListingAsync(listingId, userId.Value, cancellationToken);
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

    [HttpPost("reject-dropoff")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectDropoff(Guid listingId, string? reason, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return RedirectToAction("Login", "Auth", new { returnUrl = "/store" });
        }

        var result = await _storeService.RejectDropoffListingAsync(listingId, userId.Value, reason, cancellationToken);
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
}
