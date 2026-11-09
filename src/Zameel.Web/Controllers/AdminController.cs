using Microsoft.AspNetCore.Mvc;
using Zameel.Application.Common;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;
using Zameel.Domain.Enums;

namespace Zameel.Web.Controllers;

public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly IAcademicCatalogService _catalogService;

    public AdminController(IAdminService adminService, IAcademicCatalogService catalogService)
    {
        _adminService = adminService;
        _catalogService = catalogService;
    }

    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        if (User.Identity?.IsAuthenticated != true || (!User.IsInRole("Admin") && !User.IsInRole("ADMIN") && !User.IsInRole("Moderator") && !User.IsInRole("MODERATOR")))
        {
            TempData["ErrorMessage"] = "يرجى تسجيل الدخول بحساب مصرح له (مدير أو مشرف) للوصول إلى لوحة الإدارة.";
            context.Result = RedirectToAction("Login", "Auth", new { returnUrl = "/Admin" });
        }
    }

    // 1. Dashboard Overview
    public async Task<IActionResult> Index()
    {
        var result = await _adminService.GetDashboardStatsAsync();
        if (!result.isSuccess)
        {
            TempData["ErrorMessage"] = result.message;
            return View(null);
        }

        return View(result.data);
    }

    // 2. Student Verifications
    public async Task<IActionResult> Verifications()
    {
        var result = await _adminService.GetPendingVerificationsAsync();
        return View(result.data ?? new());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveVerification(Guid userId)
    {
        var result = await _adminService.ApproveVerificationAsync(userId);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(Verifications));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectVerification(Guid userId, string reason = "صورة غير واضحة")
    {
        var result = await _adminService.RejectVerificationAsync(userId, reason);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(Verifications));
    }

    // 3. Sold Reports Queue
    [Route("admin/soldreports")]
    [Route("admin/sold-reports")]
    public async Task<IActionResult> SoldReports()
    {
        var result = await _adminService.GetSoldReportsQueueAsync();
        return View(result.data ?? new());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmSold(Guid listingId)
    {
        var result = await _adminService.ConfirmSoldListingAsync(listingId);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(SoldReports));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DismissSoldReports(Guid listingId)
    {
        var result = await _adminService.DismissSoldReportsAsync(listingId);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(SoldReports));
    }

    // 4. Listings Moderation
    public async Task<IActionResult> Listings(Guid? universityId, ListingStatus? status, int page = 1)
    {
        var result = await _adminService.GetListingsAsync(universityId, status, page, 15);
        var universitiesResult = await _catalogService.GetActiveUniversitiesAsync();

        ViewBag.Universities = universitiesResult.data ?? new();
        ViewBag.CurrentUniversityId = universityId;
        ViewBag.CurrentStatus = status;

        return View(result.data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleListingStatus(Guid listingId, ListingStatus newStatus)
    {
        var result = await _adminService.ToggleListingStatusAsync(listingId, newStatus);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(Listings));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteListing(Guid listingId)
    {
        var result = await _adminService.DeleteListingAsync(listingId);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(Listings));
    }

    // 5. User Directory
    public async Task<IActionResult> Users(string? search, string? role, int page = 1)
    {
        var result = await _adminService.GetUsersAsync(search, role, page, 15);
        var rolesResult = await _adminService.GetRolesAsync();
        var plansResult = await _adminService.GetPlansAsync();

        ViewBag.Search = search;
        ViewBag.Role = role;
        ViewBag.Roles = rolesResult.data ?? new();
        ViewBag.Plans = plansResult.data ?? new();

        return View(result.data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(Guid userId, Guid newRoleId)
    {
        var result = await _adminService.ChangeUserRoleAsync(userId, newRoleId);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePlan(Guid userId, Guid planId)
    {
        var result = await _adminService.ChangeUserPlanAsync(userId, planId);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(Users));
    }

    // 5.1 Listing Moderation Queue
    public async Task<IActionResult> Moderation()
    {
        var result = await _adminService.GetPendingApprovalListingsAsync();
        return View(result.data ?? new());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveListing(Guid listingId)
    {
        var result = await _adminService.ApproveListingAsync(listingId);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(Moderation));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectListing(Guid listingId, string reason)
    {
        var result = await _adminService.RejectListingAsync(listingId, reason);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(Moderation));
    }

    // 5.2 Roles & Permissions Management (RBAC)
    public async Task<IActionResult> Roles()
    {
        var rolesResult = await _adminService.GetRolesAsync();
        var permissionsResult = await _adminService.GetAllPermissionsAsync();
        ViewBag.Permissions = permissionsResult.data ?? new();
        return View(rolesResult.data ?? new());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRole(CreateRoleDto dto)
    {
        var result = await _adminService.CreateRoleAsync(dto);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(Roles));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRolePermissions(UpdateRolePermissionsDto dto)
    {
        var result = await _adminService.UpdateRolePermissionsAsync(dto);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(Roles));
    }

    // 5.3 Plans Management (الباقات)
    public async Task<IActionResult> Plans()
    {
        var result = await _adminService.GetPlansAsync();
        return View(result.data ?? new());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePlan(CreatePlanDto dto)
    {
        var result = await _adminService.CreatePlanAsync(dto);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(Plans));
    }

    // 6. Stores & B2B
    public async Task<IActionResult> Stores()
    {
        var result = await _adminService.GetStoresAsync();
        return View(result.data ?? new());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveStore(Guid storeProfileId, int months = 3)
    {
        var result = await _adminService.ApproveStoreAsync(storeProfileId, months);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(Stores));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FreezeStore(Guid storeProfileId)
    {
        var result = await _adminService.FreezeStoreAsync(storeProfileId);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(Stores));
    }

    // ===================================================================
    // 7. Universities Management
    // ===================================================================
    [HttpGet]
    [Route("admin/universities")]
    public async Task<IActionResult> Universities(CancellationToken cancellationToken)
    {
        var result = await _adminService.GetUniversitiesAsync(cancellationToken);
        return View(result.data ?? new());
    }

    [HttpGet]
    [Route("admin/universities/{id:guid}")]
    public async Task<IActionResult> UniversityDetails(Guid id, CancellationToken cancellationToken)
    {
        var result = await _adminService.GetUniversityDetailAsync(id, cancellationToken);
        if (!result.isSuccess || result.data == null)
        {
            TempData["ErrorMessage"] = result.message;
            return RedirectToAction(nameof(Universities));
        }

        return View(result.data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/universities/add")]
    public async Task<IActionResult> AddUniversity(CreateUniversityRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _adminService.CreateUniversityAsync(request, cancellationToken);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(Universities));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/universities/update")]
    public async Task<IActionResult> UpdateUniversity(UpdateUniversityRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _adminService.UpdateUniversityAsync(request, cancellationToken);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(UniversityDetails), new { id = request.id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/universities/toggle-status")]
    public async Task<IActionResult> ToggleUniversityStatus(Guid id, CancellationToken cancellationToken)
    {
        var result = await _adminService.ToggleUniversityStatusAsync(id, cancellationToken);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(Universities));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/universities/link-faculty")]
    public async Task<IActionResult> LinkFacultyToUniversity(LinkFacultyToUniversityRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _adminService.LinkFacultyToUniversityAsync(request, cancellationToken);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(UniversityDetails), new { id = request.universityId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/universities/unlink-faculty")]
    public async Task<IActionResult> UnlinkFacultyFromUniversity(Guid universityFacultyId, Guid universityId, CancellationToken cancellationToken)
    {
        var result = await _adminService.UnlinkFacultyFromUniversityAsync(universityFacultyId, cancellationToken);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(UniversityDetails), new { id = universityId });
    }

    // ===================================================================
    // 8. Faculties & Colleges Master Catalog
    // ===================================================================
    [HttpGet]
    [Route("admin/faculties")]
    public async Task<IActionResult> Faculties(CancellationToken cancellationToken)
    {
        var result = await _adminService.GetFacultiesAsync(cancellationToken);
        return View(result.data ?? new());
    }

    [HttpGet]
    [Route("admin/faculties/{id:guid}")]
    public async Task<IActionResult> FacultyDetails(Guid id, CancellationToken cancellationToken)
    {
        var result = await _adminService.GetFacultyDetailAsync(id, cancellationToken);
        if (!result.isSuccess || result.data == null)
        {
            TempData["ErrorMessage"] = result.message;
            return RedirectToAction(nameof(Faculties));
        }

        return View(result.data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/faculties/add")]
    public async Task<IActionResult> AddFaculty(CreateFacultyRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _adminService.CreateFacultyAsync(request, cancellationToken);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(Faculties));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/faculties/update")]
    public async Task<IActionResult> UpdateFaculty(UpdateFacultyRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _adminService.UpdateFacultyAsync(request, cancellationToken);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(FacultyDetails), new { id = request.id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/faculties/toggle-status")]
    public async Task<IActionResult> ToggleFacultyStatus(Guid id, CancellationToken cancellationToken)
    {
        var result = await _adminService.ToggleFacultyStatusAsync(id, cancellationToken);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(Faculties));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/faculties/link-major")]
    public async Task<IActionResult> LinkMajorToFaculty(LinkMajorToFacultyRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _adminService.LinkMajorToFacultyAsync(request, cancellationToken);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(FacultyDetails), new { id = request.facultyId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/faculties/unlink-major")]
    public async Task<IActionResult> UnlinkMajorFromFaculty(Guid facultyId, Guid majorId, CancellationToken cancellationToken)
    {
        var result = await _adminService.UnlinkMajorFromFacultyAsync(facultyId, majorId, cancellationToken);
        if (result.isSuccess)
            TempData["SuccessMessage"] = result.message;
        else
            TempData["ErrorMessage"] = result.message;

        return RedirectToAction(nameof(FacultyDetails), new { id = facultyId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/majors/add")]
    public async Task<IActionResult> AddMajor(CreateMajorRequestDto request, Guid? returnToFacultyId, CancellationToken cancellationToken)
    {
        var result = await _adminService.CreateMajorAsync(request, cancellationToken);
        if (result.isSuccess)
        {
            TempData["SuccessMessage"] = result.message;

            // If created from within faculty details, link it automatically!
            if (returnToFacultyId.HasValue && returnToFacultyId.Value != Guid.Empty)
            {
                await _adminService.LinkMajorToFacultyAsync(new LinkMajorToFacultyRequestDto(returnToFacultyId.Value, result.data), cancellationToken);
                return RedirectToAction(nameof(FacultyDetails), new { id = returnToFacultyId.Value });
            }
        }
        else
        {
            TempData["ErrorMessage"] = result.message;
        }

        if (returnToFacultyId.HasValue && returnToFacultyId.Value != Guid.Empty)
        {
            return RedirectToAction(nameof(FacultyDetails), new { id = returnToFacultyId.Value });
        }

        return RedirectToAction(nameof(Faculties));
    }
}
