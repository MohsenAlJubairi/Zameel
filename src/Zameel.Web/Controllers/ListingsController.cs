using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zameel.Application.Common;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;
using Zameel.Domain.Constants;
using Zameel.Domain.Enums;
using Zameel.Web.Models;

namespace Zameel.Web.Controllers;

[Route("listings")]
public class ListingsController : Controller
{
    private readonly IListingService _listingService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAcademicCatalogService _catalogService;
    private readonly IStoreService _storeService;
    private readonly IApplicationDbContext _context;

    public ListingsController(
        IListingService listingService,
        IFileStorageService fileStorageService,
        IAcademicCatalogService catalogService,
        IStoreService storeService,
        IApplicationDbContext context)
    {
        _listingService = listingService;
        _fileStorageService = fileStorageService;
        _catalogService = catalogService;
        _storeService = storeService;
        _context = context;
    }

    private Guid? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out var id) ? id : null;
    }

    [HttpGet("{id:guid}")]
    [HttpGet("details/{id:guid}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        var result = await _listingService.GetByIdAsync(id, currentUserId, cancellationToken);
        if (!result.isSuccess || result.data == null)
        {
            return NotFound();
        }

        ViewData["OgTitle"] = result.data.title;
        ViewData["OgDescription"] = $"{result.data.universityName} • السعر: {(result.data.price > 0 ? result.data.price.ToString("N0") + " ريال يمني" : "مجاناً")}";
        if (result.data.imageUrls.Count > 0)
        {
            ViewData["OgImage"] = result.data.imageUrls[0];
        }
        else
        {
            ViewData["OgImage"] = "/images/zameel-brand-icon.png";
        }

        return View(result.data);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            return RedirectToAction("Login", "Auth", new { returnUrl = "/listings/create" });
        }

        var universitiesResult = await _catalogService.GetActiveUniversitiesAsync(cancellationToken);

        var currentUser = await _context.users
            .AsNoTracking()
            .Include(u => u.role)
            .FirstOrDefaultAsync(u => u.id == currentUserId.Value, cancellationToken);

        var isStore = currentUser?.role?.normalizedName == SystemRoles.Store;
        var availableStores = new List<StoreDropdownDto>();
        if (!isStore && currentUser?.universityId != null && currentUser.universityId.Value != Guid.Empty)
        {
            var storesResult = await _storeService.GetApprovedStoresByUniversityAsync(currentUser.universityId.Value, cancellationToken);
            availableStores = storesResult.data ?? new();
        }

        var model = new CreateListingViewModel
        {
            universities = universitiesResult.data ?? new List<UniversityDto>(),
            universityId = currentUser?.universityId ?? Guid.Empty,
            primaryFacultyId = currentUser?.facultyId,
            primaryMajorId = currentUser?.majorId,
            isStoreUser = isStore,
            availableStores = availableStores
        };

        return View(model);
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateListingViewModel model, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            return RedirectToAction("Login", "Auth", new { returnUrl = "/listings/create" });
        }

        // Auto-zero price on donation
        if (model.type == ListingType.Donation)
        {
            model.price = 0;
        }

        if (string.IsNullOrWhiteSpace(model.title) || string.IsNullOrWhiteSpace(model.description))
        {
            var unis = await _catalogService.GetActiveUniversitiesAsync(cancellationToken);
            var stores = await _storeService.GetApprovedStoresByUniversityAsync(model.universityId, cancellationToken);
            model.universities = unis.data ?? new List<UniversityDto>();
            model.availableStores = stores.data ?? new List<StoreDropdownDto>();
            model.errorMessage = "يرجى تعبئة كافة الحقول المطلوبة (اسم المستلزم وتفاصيله).";
            return View(model);
        }

        // Retrieve current authenticated user
        var user = await _context.users
            .Include(u => u.role)
            .FirstOrDefaultAsync(u => u.id == currentUserId.Value, cancellationToken);
        if (user == null)
        {
            return RedirectToAction("Login", "Auth", new { returnUrl = "/listings/create" });
        }

        var isStore = user.role?.normalizedName == SystemRoles.Store;

        // Mandatory Campus Bookstore Drop-off Point validation for normal users
        if (!isStore && (!model.pickupStoreId.HasValue || model.pickupStoreId.Value == Guid.Empty))
        {
            var unis = await _catalogService.GetActiveUniversitiesAsync(cancellationToken);
            var stores = await _storeService.GetApprovedStoresByUniversityAsync(model.universityId, cancellationToken);
            model.universities = unis.data ?? new List<UniversityDto>();
            model.availableStores = stores.data ?? new List<StoreDropdownDto>();
            model.isStoreUser = isStore;
            model.errorMessage = "يرجى اختيار إحدى المكتبات الجامعية المعتمدة كنقطة إيداع واستلام للسلعة.";
            return View(model);
        }

        // Upload images if any
        var imageUrls = new List<string>();
        if (model.images != null && model.images.Count > 0)
        {
            foreach (var img in model.images.Take(5))
            {
                if (img.Length > 0)
                {
                    using var stream = img.OpenReadStream();
                    var uploadResult = await _fileStorageService.SaveFileAsync(stream, img.FileName, "listings", cancellationToken);
                    if (uploadResult.isSuccess && !string.IsNullOrEmpty(uploadResult.data))
                    {
                        imageUrls.Add(uploadResult.data);
                    }
                }
            }
        }

        var dto = new CreateListingDto(
            model.universityId,
            model.primaryFacultyId,
            model.primaryMajorId,
            model.title,
            model.description,
            model.price,
            model.type,
            model.condition,
            model.scope,
            targetFacultyIds: null,
            targetMajorIds: null,
            pickupStoreId: model.pickupStoreId
        );

        var createResult = await _listingService.CreateListingAsync(dto, imageUrls, user.id, cancellationToken);
        if (!createResult.isSuccess)
        {
            var unis = await _catalogService.GetActiveUniversitiesAsync(cancellationToken);
            var stores = await _storeService.GetApprovedStoresByUniversityAsync(model.universityId, cancellationToken);
            model.universities = unis.data ?? new List<UniversityDto>();
            model.availableStores = stores.data ?? new List<StoreDropdownDto>();
            model.isStoreUser = isStore;
            model.errorMessage = createResult.message;
            return View(model);
        }

        TempData["SuccessMessage"] = createResult.message;
        if (!isStore)
        {
            // Student's item is deposited and pending bookstore confirmation; route to profile to monitor status
            return RedirectToAction("Index", "Profile");
        }

        return RedirectToAction(nameof(Details), new { id = createResult.data });
    }

    // JSON API Endpoints for Mobile / AJAX
    [HttpGet("api/feed")]
    public async Task<IActionResult> GetFeedApi([FromQuery] ListingFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await _listingService.GetFeedAsync(filter, cancellationToken);
        return Json(result);
    }

    [HttpPost("api/upload-image")]
    public async Task<IActionResult> UploadImage(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return Json(ServiceResult<string>.Fail(AppMessages.InvalidImageFormat));
        }

        using var stream = file.OpenReadStream();
        var result = await _fileStorageService.SaveFileAsync(stream, file.FileName, "listings", cancellationToken);
        return Json(result);
    }

    [HttpPost("{id:guid}/mark-sold")]
    public async Task<IActionResult> MarkSold(Guid id, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            return Json(ServiceResult<bool>.Fail(AppMessages.UnauthorizedAccess));
        }

        var result = await _listingService.MarkAsSoldAsync(id, currentUserId.Value, cancellationToken);
        return Json(result);
    }

    [HttpPost("{id:guid}/renew")]
    public async Task<IActionResult> Renew(Guid id, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            return Json(ServiceResult<bool>.Fail(AppMessages.UnauthorizedAccess));
        }

        var result = await _listingService.RenewListingAsync(id, currentUserId.Value, cancellationToken);
        return Json(result);
    }

    [HttpPost("{id:guid}/report-sold")]
    public async Task<IActionResult> ReportSold(Guid id, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();
        if (!currentUserId.HasValue)
        {
            return Json(ServiceResult<bool>.Fail("يرجى تسجيل الدخول أولاً للإبلاغ عن المستلزم."));
        }

        var result = await _listingService.ReportSoldAsync(id, currentUserId.Value, cancellationToken);
        return Json(result);
    }
}
