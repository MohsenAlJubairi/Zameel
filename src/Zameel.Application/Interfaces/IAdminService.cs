using Zameel.Application.Common;
using Zameel.Application.DTOs;
using Zameel.Domain.Enums;

namespace Zameel.Application.Interfaces;

public interface IAdminService
{
    Task<ServiceResult<AdminDashboardStatsDto>> GetDashboardStatsAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<List<PendingVerificationDto>>> GetPendingVerificationsAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> ApproveVerificationAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> RejectVerificationAsync(Guid userId, string reason, CancellationToken cancellationToken = default);

    Task<ServiceResult<List<SoldReportItemDto>>> GetSoldReportsQueueAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> ConfirmSoldListingAsync(Guid listingId, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> DismissSoldReportsAsync(Guid listingId, CancellationToken cancellationToken = default);

    Task<ServiceResult<PagedResult<AdminListingItemDto>>> GetListingsAsync(
        Guid? universityId = null,
        ListingStatus? status = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> ToggleListingStatusAsync(Guid listingId, ListingStatus newStatus, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> DeleteListingAsync(Guid listingId, CancellationToken cancellationToken = default);

    Task<ServiceResult<PagedResult<AdminUserItemDto>>> GetUsersAsync(
        string? search = null,
        string? role = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> ChangeUserRoleAsync(Guid userId, Guid newRoleId, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> ChangeUserRoleByNameAsync(Guid userId, string newRoleNormalized, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> ChangeUserPlanAsync(Guid userId, Guid planId, CancellationToken cancellationToken = default);

    // Listing Moderation & Approval
    Task<ServiceResult<List<AdminListingItemDto>>> GetPendingApprovalListingsAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> ApproveListingAsync(Guid listingId, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> RejectListingAsync(Guid listingId, string reason, CancellationToken cancellationToken = default);

    // Roles & Permissions Management
    Task<ServiceResult<List<RoleDto>>> GetRolesAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<RoleDto>> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    Task<ServiceResult<Guid>> CreateRoleAsync(CreateRoleDto dto, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> UpdateRolePermissionsAsync(UpdateRolePermissionsDto dto, CancellationToken cancellationToken = default);

    Task<ServiceResult<List<PermissionDto>>> GetAllPermissionsAsync(CancellationToken cancellationToken = default);

    // Plans / Packages Management (الباقات)
    Task<ServiceResult<List<PlanDto>>> GetPlansAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<Guid>> CreatePlanAsync(CreatePlanDto dto, CancellationToken cancellationToken = default);

    Task<ServiceResult<List<AdminStoreItemDto>>> GetStoresAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> ApproveStoreAsync(Guid storeProfileId, int subscriptionMonths = 3, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> FreezeStoreAsync(Guid storeProfileId, CancellationToken cancellationToken = default);

    // Academic Catalog: Universities Management
    Task<ServiceResult<List<AdminUniversityItemDto>>> GetUniversitiesAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<AdminUniversityDetailDto>> GetUniversityDetailAsync(Guid universityId, CancellationToken cancellationToken = default);

    Task<ServiceResult<Guid>> CreateUniversityAsync(CreateUniversityRequestDto request, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> UpdateUniversityAsync(UpdateUniversityRequestDto request, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> ToggleUniversityStatusAsync(Guid universityId, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> LinkFacultyToUniversityAsync(LinkFacultyToUniversityRequestDto request, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> UnlinkFacultyFromUniversityAsync(Guid universityFacultyId, CancellationToken cancellationToken = default);

    // Academic Catalog: Faculties Master Catalog
    Task<ServiceResult<List<AdminFacultyItemDto>>> GetFacultiesAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<AdminFacultyDetailDto>> GetFacultyDetailAsync(Guid facultyId, CancellationToken cancellationToken = default);

    Task<ServiceResult<Guid>> CreateFacultyAsync(CreateFacultyRequestDto request, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> UpdateFacultyAsync(UpdateFacultyRequestDto request, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> ToggleFacultyStatusAsync(Guid facultyId, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> LinkMajorToFacultyAsync(LinkMajorToFacultyRequestDto request, CancellationToken cancellationToken = default);

    Task<ServiceResult<bool>> UnlinkMajorFromFacultyAsync(Guid facultyId, Guid majorId, CancellationToken cancellationToken = default);

    // Academic Catalog: Majors Master Catalog
    Task<ServiceResult<List<AdminMajorItemDto>>> GetMajorsAsync(CancellationToken cancellationToken = default);

    Task<ServiceResult<Guid>> CreateMajorAsync(CreateMajorRequestDto request, CancellationToken cancellationToken = default);
}
