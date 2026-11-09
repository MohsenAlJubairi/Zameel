using Microsoft.EntityFrameworkCore;
using Zameel.Application.Common;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;
using Zameel.Domain.Constants;
using Zameel.Domain.Entities;
using Zameel.Domain.Enums;

namespace Zameel.Application.Services;

public class AdminService : IAdminService
{
    private readonly IApplicationDbContext _context;
    private readonly IQuotaService _quotaService;

    public AdminService(IApplicationDbContext context, IQuotaService quotaService)
    {
        _context = context;
        _quotaService = quotaService;
    }

    public async Task<ServiceResult<AdminDashboardStatsDto>> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        var totalActiveListings = await _context.listings
            .AsNoTracking()
            .CountAsync(l => l.status == ListingStatus.Active, cancellationToken);

        var pendingVerificationsCount = await _context.users
            .AsNoTracking()
            .CountAsync(u => !u.isVerified && !string.IsNullOrEmpty(u.studentRegNo), cancellationToken);

        var pendingSoldReportsCount = await _context.listings
            .AsNoTracking()
            .CountAsync(l => l.status == ListingStatus.Active && l.reportedSoldCount >= 2, cancellationToken);

        var totalVerifiedStudents = await _context.users
            .AsNoTracking()
            .CountAsync(u => u.isVerified, cancellationToken);

        var totalDealsConfirmed = await _context.listings
            .AsNoTracking()
            .CountAsync(l => l.status == ListingStatus.Sold, cancellationToken);

        var totalUsers = await _context.users
            .AsNoTracking()
            .CountAsync(cancellationToken);

        // Recent Verifications (top 5)
        var recentVerifications = await _context.users
            .AsNoTracking()
            .Where(u => !u.isVerified && !string.IsNullOrEmpty(u.studentRegNo))
            .OrderByDescending(u => u.createdAt)
            .Take(5)
            .Select(u => new PendingVerificationDto(
                u.id,
                u.fullName,
                u.email,
                u.phone,
                u.university != null ? u.university.name : null,
                u.faculty != null ? u.faculty.name : null,
                u.major != null ? u.major.name : null,
                u.studentRegNo,
                u.createdAt
            ))
            .ToListAsync(cancellationToken);

        // Recent Sold Reports (top 5)
        var recentSoldReports = await _context.listings
            .AsNoTracking()
            .Where(l => l.status == ListingStatus.Active && l.reportedSoldCount > 0)
            .OrderByDescending(l => l.reportedSoldCount)
            .Take(5)
            .Select(l => new SoldReportItemDto(
                l.id,
                l.title,
                l.price,
                l.user.fullName,
                l.user.email,
                l.reportedSoldCount,
                l.createdAt,
                l.reports.Select(r => r.reporter.fullName).ToList()
            ))
            .ToListAsync(cancellationToken);

        var stats = new AdminDashboardStatsDto(
            totalActiveListings,
            pendingVerificationsCount,
            pendingSoldReportsCount,
            totalVerifiedStudents,
            totalDealsConfirmed,
            totalUsers,
            recentVerifications,
            recentSoldReports
        );

        return ServiceResult<AdminDashboardStatsDto>.Success(stats);
    }

    public async Task<ServiceResult<List<PendingVerificationDto>>> GetPendingVerificationsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _context.users
            .AsNoTracking()
            .Where(u => !u.isVerified && !string.IsNullOrEmpty(u.studentRegNo))
            .OrderByDescending(u => u.createdAt)
            .Select(u => new PendingVerificationDto(
                u.id,
                u.fullName,
                u.email,
                u.phone,
                u.university != null ? u.university.name : null,
                u.faculty != null ? u.faculty.name : null,
                u.major != null ? u.major.name : null,
                u.studentRegNo,
                u.createdAt
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<PendingVerificationDto>>.Success(items);
    }

    public async Task<ServiceResult<bool>> ApproveVerificationAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.users
            .Include(u => u.role)
            .FirstOrDefaultAsync(u => u.id == userId, cancellationToken);
        if (user == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        user.isVerified = true;
        user.verifiedAt = DateTime.UtcNow;

        if (user.role.normalizedName == SystemRoles.StudentUnverified)
        {
            var verifiedRole = await _context.roles
                .FirstOrDefaultAsync(r => r.normalizedName == SystemRoles.StudentVerified, cancellationToken);
            if (verifiedRole != null)
            {
                user.roleId = verifiedRole.id;
            }

            var verifiedPlan = await _context.plans
                .FirstOrDefaultAsync(p => p.hasVerifiedBadge && p.isActive, cancellationToken);
            if (verifiedPlan != null)
            {
                user.planId = verifiedPlan.id;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(true, AppMessages.AccountVerifiedSuccess);
    }

    public async Task<ServiceResult<bool>> RejectVerificationAsync(Guid userId, string reason, CancellationToken cancellationToken = default)
    {
        var user = await _context.users.FirstOrDefaultAsync(u => u.id == userId, cancellationToken);
        if (user == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        user.isVerified = false;
        user.studentRegNo = null; // Reset so they can re-enter their registration number
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true, "تم رفض طلب التوثيق وإتاحة إعادة إدخال البيانات للطالب.");
    }

    public async Task<ServiceResult<List<SoldReportItemDto>>> GetSoldReportsQueueAsync(CancellationToken cancellationToken = default)
    {
        var items = await _context.listings
            .AsNoTracking()
            .Where(l => l.status == ListingStatus.Active && l.reportedSoldCount > 0)
            .OrderByDescending(l => l.reportedSoldCount)
            .ThenByDescending(l => l.createdAt)
            .Select(l => new SoldReportItemDto(
                l.id,
                l.title,
                l.price,
                l.user.fullName,
                l.user.email,
                l.reportedSoldCount,
                l.createdAt,
                l.reports.Select(r => r.reporter.fullName).ToList()
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<SoldReportItemDto>>.Success(items);
    }

    public async Task<ServiceResult<bool>> ConfirmSoldListingAsync(Guid listingId, CancellationToken cancellationToken = default)
    {
        var listing = await _context.listings.FirstOrDefaultAsync(l => l.id == listingId, cancellationToken);
        if (listing == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        listing.status = ListingStatus.Sold;
        listing.soldAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        await _quotaService.SyncUserActiveListingsCountAsync(listing.userId, cancellationToken);

        return ServiceResult<bool>.Success(true, AppMessages.ListingMarkedAsSold);
    }

    public async Task<ServiceResult<bool>> DismissSoldReportsAsync(Guid listingId, CancellationToken cancellationToken = default)
    {
        var listing = await _context.listings
            .Include(l => l.reports)
            .FirstOrDefaultAsync(l => l.id == listingId, cancellationToken);

        if (listing == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        listing.reportedSoldCount = 0;
        foreach (var r in listing.reports.Where(r => r.reason == ReportReason.ItemSold))
        {
            r.isProcessed = true;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(true, "تم تجاهل البلاغات بنجاح.");
    }

    public async Task<ServiceResult<PagedResult<AdminListingItemDto>>> GetListingsAsync(
        Guid? universityId = null,
        ListingStatus? status = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        var query = _context.listings.AsNoTracking();

        if (universityId.HasValue)
            query = query.Where(l => l.universityId == universityId.Value);

        if (status.HasValue)
            query = query.Where(l => l.status == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(l => l.createdAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new AdminListingItemDto(
                l.id,
                l.title,
                l.price,
                l.type,
                l.status,
                l.user.fullName,
                l.university.name,
                l.primaryFaculty != null ? l.primaryFaculty.name : null,
                l.viewsCount,
                l.reportedSoldCount,
                l.createdAt
            ))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<AdminListingItemDto>(items, totalCount, page, pageSize);
        return ServiceResult<PagedResult<AdminListingItemDto>>.Success(result);
    }

    public async Task<ServiceResult<bool>> ToggleListingStatusAsync(Guid listingId, ListingStatus newStatus, CancellationToken cancellationToken = default)
    {
        var listing = await _context.listings.FirstOrDefaultAsync(l => l.id == listingId, cancellationToken);
        if (listing == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        listing.status = newStatus;
        if (newStatus == ListingStatus.Sold)
            listing.soldAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        await _quotaService.SyncUserActiveListingsCountAsync(listing.userId, cancellationToken);

        return ServiceResult<bool>.Success(true, "تم تحديث حالة الإعلان بنجاح.");
    }

    public async Task<ServiceResult<bool>> DeleteListingAsync(Guid listingId, CancellationToken cancellationToken = default)
    {
        var listing = await _context.listings.FirstOrDefaultAsync(l => l.id == listingId, cancellationToken);
        if (listing == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        listing.isDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        await _quotaService.SyncUserActiveListingsCountAsync(listing.userId, cancellationToken);

        return ServiceResult<bool>.Success(true, "تم حذف الإعلان بنجاح.");
    }

    public async Task<ServiceResult<PagedResult<AdminUserItemDto>>> GetUsersAsync(
        string? search = null,
        string? role = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        var query = _context.users
            .AsNoTracking()
            .Include(u => u.role)
            .Include(u => u.plan)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(u => u.fullName.Contains(term) || u.email.Contains(term) || (u.phone != null && u.phone.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            var roleTerm = role.Trim().ToUpperInvariant();
            query = query.Where(u => u.role.normalizedName == roleTerm || u.role.name == role);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(u => u.createdAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new AdminUserItemDto(
                u.id,
                u.fullName,
                u.email,
                u.phone,
                u.role.name,
                u.role.normalizedName,
                u.isVerified,
                u.university != null ? u.university.name : null,
                u.activeListingsCount,
                u.createdAt,
                u.plan != null ? u.plan.name : null
            ))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<AdminUserItemDto>(items, totalCount, page, pageSize);
        return ServiceResult<PagedResult<AdminUserItemDto>>.Success(result);
    }

    public async Task<ServiceResult<bool>> ChangeUserRoleAsync(Guid userId, Guid newRoleId, CancellationToken cancellationToken = default)
    {
        var user = await _context.users.FirstOrDefaultAsync(u => u.id == userId, cancellationToken);
        if (user == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        var targetRole = await _context.roles.FirstOrDefaultAsync(r => r.id == newRoleId, cancellationToken);
        if (targetRole == null)
            return ServiceResult<bool>.Fail("الرتبة المحددة غير موجودة.");

        user.roleId = newRoleId;
        if (targetRole.normalizedName == SystemRoles.StudentVerified)
        {
            user.isVerified = true;
            user.verifiedAt ??= DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(true, $"تم تعديل رتبة وصلاحية المستخدم بنجاح إلى: {targetRole.name}");
    }

    public async Task<ServiceResult<bool>> ChangeUserRoleByNameAsync(Guid userId, string newRoleNormalized, CancellationToken cancellationToken = default)
    {
        var role = await _context.roles
            .FirstOrDefaultAsync(r => r.normalizedName == newRoleNormalized.Trim().ToUpperInvariant(), cancellationToken);
        if (role == null)
            return ServiceResult<bool>.Fail("الرتبة المحددة غير موجودة.");

        return await ChangeUserRoleAsync(userId, role.id, cancellationToken);
    }

    public async Task<ServiceResult<bool>> ChangeUserPlanAsync(Guid userId, Guid planId, CancellationToken cancellationToken = default)
    {
        var user = await _context.users.FirstOrDefaultAsync(u => u.id == userId, cancellationToken);
        if (user == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        var plan = await _context.plans.FirstOrDefaultAsync(p => p.id == planId, cancellationToken);
        if (plan == null)
            return ServiceResult<bool>.Fail("الباقة المحددة غير موجودة.");

        user.planId = planId;
        user.planExpiresAt = plan.durationDays > 0 ? DateTime.UtcNow.AddDays(plan.durationDays) : null;
        if (plan.hasVerifiedBadge)
        {
            user.isVerified = true;
            user.verifiedAt ??= DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(true, $"تم تحديث باقة المستخدم بنجاح إلى: {plan.name}");
    }

    public async Task<ServiceResult<List<AdminStoreItemDto>>> GetStoresAsync(CancellationToken cancellationToken = default)
    {
        var items = await _context.storeProfiles
            .AsNoTracking()
            .Include(s => s.user)
            .OrderByDescending(s => s.createdAt)
            .Select(s => new AdminStoreItemDto(
                s.id,
                s.userId,
                s.storeName,
                s.user.fullName,
                s.user.email,
                s.commercialRegister,
                s.isApproved,
                s.subscriptionExpiresAt,
                s.createdAt
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<AdminStoreItemDto>>.Success(items);
    }

    public async Task<ServiceResult<bool>> ApproveStoreAsync(Guid storeProfileId, int subscriptionMonths = 3, CancellationToken cancellationToken = default)
    {
        var store = await _context.storeProfiles
            .Include(s => s.user)
            .FirstOrDefaultAsync(s => s.id == storeProfileId, cancellationToken);

        if (store == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        store.isApproved = true;
        store.subscriptionExpiresAt = DateTime.UtcNow.AddMonths(subscriptionMonths);

        var storeRole = await _context.roles
            .FirstOrDefaultAsync(r => r.normalizedName == SystemRoles.Store, cancellationToken);
        if (storeRole != null)
        {
            store.user.roleId = storeRole.id;
        }

        var storePlan = await _context.plans
            .FirstOrDefaultAsync(p => p.hasStoreProfile && p.isActive, cancellationToken);
        if (storePlan != null)
        {
            store.user.planId = storePlan.id;
            store.user.planExpiresAt = store.subscriptionExpiresAt;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(true, "تم اعتماد وتفعيل الحساب التجاري للمتجر بنجاح.");
    }

    public async Task<ServiceResult<bool>> FreezeStoreAsync(Guid storeProfileId, CancellationToken cancellationToken = default)
    {
        var store = await _context.storeProfiles
            .Include(s => s.user)
            .FirstOrDefaultAsync(s => s.id == storeProfileId, cancellationToken);

        if (store == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        store.isApproved = false;
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true, "تم تجميد حساب المتجر.");
    }

    // ===================================================================
    // Academic Catalog: Universities Management
    // ===================================================================
    public async Task<ServiceResult<List<AdminUniversityItemDto>>> GetUniversitiesAsync(CancellationToken cancellationToken = default)
    {
        var items = await _context.universities
            .AsNoTracking()
            .OrderBy(u => u.name)
            .Select(u => new AdminUniversityItemDto(
                u.id,
                u.name,
                u.city,
                u.logoUrl,
                u.isActive,
                u.universityFaculties.Count(uf => !uf.isDeleted),
                u.users.Count(usr => !usr.isDeleted),
                u.listings.Count(l => !l.isDeleted),
                u.createdAt
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<AdminUniversityItemDto>>.Success(items);
    }

    public async Task<ServiceResult<AdminUniversityDetailDto>> GetUniversityDetailAsync(Guid universityId, CancellationToken cancellationToken = default)
    {
        var uni = await _context.universities
            .AsNoTracking()
            .Where(u => u.id == universityId)
            .Select(u => new
            {
                u.id,
                u.name,
                u.city,
                u.logoUrl,
                u.isActive,
                studentsCount = u.users.Count(usr => !usr.isDeleted),
                listingsCount = u.listings.Count(l => !l.isDeleted),
                faculties = u.universityFaculties
                    .Where(uf => !uf.isDeleted)
                    .OrderBy(uf => uf.customName ?? uf.faculty.name)
                    .Select(uf => new AdminUniversityFacultyDto(
                        uf.id,
                        uf.faculty.id,
                        uf.faculty.name,
                        uf.customName,
                        uf.faculty.slug,
                        uf.isActive,
                        uf.faculty.facultyMajors.Count(fm => !fm.isDeleted),
                        uf.faculty.primaryListings.Count(l => !l.isDeleted && l.universityId == u.id)
                    )).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (uni == null)
            return ServiceResult<AdminUniversityDetailDto>.Fail(AppMessages.ItemNotFound);

        var linkedFacultyIds = uni.faculties.Select(f => f.facultyId).ToHashSet();
        var availableFaculties = await _context.faculties
            .AsNoTracking()
            .Where(f => !linkedFacultyIds.Contains(f.id) && f.isActive)
            .OrderBy(f => f.name)
            .Select(f => new FacultyDto(f.id, f.name, f.slug, f.name))
            .ToListAsync(cancellationToken);

        var result = new AdminUniversityDetailDto(
            uni.id,
            uni.name,
            uni.city,
            uni.logoUrl,
            uni.isActive,
            uni.studentsCount,
            uni.listingsCount,
            uni.faculties,
            availableFaculties
        );

        return ServiceResult<AdminUniversityDetailDto>.Success(result);
    }

    public async Task<ServiceResult<Guid>> CreateUniversityAsync(CreateUniversityRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.name) || string.IsNullOrWhiteSpace(request.city))
            return ServiceResult<Guid>.Fail("يرجى إدخال اسم الجامعة والمدينة.");

        var exists = await _context.universities.AnyAsync(u => u.name.ToLower() == request.name.Trim().ToLower(), cancellationToken);
        if (exists)
            return ServiceResult<Guid>.Fail(AppMessages.DuplicateAcademicEntry);

        var uni = new University
        {
            name = request.name.Trim(),
            city = request.city.Trim(),
            logoUrl = string.IsNullOrWhiteSpace(request.logoUrl) ? null : request.logoUrl.Trim(),
            isActive = true
        };

        await _context.universities.AddAsync(uni, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<Guid>.Success(uni.id, AppMessages.UniversityCreatedSuccess);
    }

    public async Task<ServiceResult<bool>> UpdateUniversityAsync(UpdateUniversityRequestDto request, CancellationToken cancellationToken = default)
    {
        var uni = await _context.universities.FirstOrDefaultAsync(u => u.id == request.id, cancellationToken);
        if (uni == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        uni.name = request.name.Trim();
        uni.city = request.city.Trim();
        if (!string.IsNullOrWhiteSpace(request.logoUrl))
            uni.logoUrl = request.logoUrl.Trim();
        uni.isActive = request.isActive;

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(true, AppMessages.UniversityUpdatedSuccess);
    }

    public async Task<ServiceResult<bool>> ToggleUniversityStatusAsync(Guid universityId, CancellationToken cancellationToken = default)
    {
        var uni = await _context.universities.FirstOrDefaultAsync(u => u.id == universityId, cancellationToken);
        if (uni == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        uni.isActive = !uni.isActive;
        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(uni.isActive, AppMessages.UniversityStatusToggled);
    }

    public async Task<ServiceResult<bool>> LinkFacultyToUniversityAsync(LinkFacultyToUniversityRequestDto request, CancellationToken cancellationToken = default)
    {
        var uniExists = await _context.universities.AnyAsync(u => u.id == request.universityId, cancellationToken);
        var facExists = await _context.faculties.AnyAsync(f => f.id == request.facultyId, cancellationToken);
        if (!uniExists || !facExists)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        var existingLink = await _context.universityFaculties
            .FirstOrDefaultAsync(uf => uf.universityId == request.universityId && uf.facultyId == request.facultyId, cancellationToken);

        if (existingLink != null)
        {
            if (existingLink.isDeleted)
            {
                existingLink.isDeleted = false;
                existingLink.isActive = true;
                if (!string.IsNullOrWhiteSpace(request.customName))
                    existingLink.customName = request.customName.Trim();
                await _context.SaveChangesAsync(cancellationToken);
                return ServiceResult<bool>.Success(true, AppMessages.FacultyLinkedSuccess);
            }
            return ServiceResult<bool>.Fail(AppMessages.DuplicateAcademicEntry);
        }

        var link = new UniversityFaculty
        {
            universityId = request.universityId,
            facultyId = request.facultyId,
            customName = string.IsNullOrWhiteSpace(request.customName) ? null : request.customName.Trim(),
            isActive = true
        };

        await _context.universityFaculties.AddAsync(link, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true, AppMessages.FacultyLinkedSuccess);
    }

    public async Task<ServiceResult<bool>> UnlinkFacultyFromUniversityAsync(Guid universityFacultyId, CancellationToken cancellationToken = default)
    {
        var link = await _context.universityFaculties.FirstOrDefaultAsync(uf => uf.id == universityFacultyId, cancellationToken);
        if (link == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        link.isDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(true, AppMessages.FacultyUnlinkedSuccess);
    }

    // ===================================================================
    // Academic Catalog: Faculties Master Catalog
    // ===================================================================
    public async Task<ServiceResult<List<AdminFacultyItemDto>>> GetFacultiesAsync(CancellationToken cancellationToken = default)
    {
        var items = await _context.faculties
            .AsNoTracking()
            .OrderBy(f => f.name)
            .Select(f => new AdminFacultyItemDto(
                f.id,
                f.name,
                f.slug,
                f.isActive,
                f.universityFaculties.Count(uf => !uf.isDeleted),
                f.facultyMajors.Count(fm => !fm.isDeleted),
                f.createdAt
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<AdminFacultyItemDto>>.Success(items);
    }

    public async Task<ServiceResult<AdminFacultyDetailDto>> GetFacultyDetailAsync(Guid facultyId, CancellationToken cancellationToken = default)
    {
        var fac = await _context.faculties
            .AsNoTracking()
            .Where(f => f.id == facultyId)
            .Select(f => new
            {
                f.id,
                f.name,
                f.slug,
                f.isActive,
                universitiesCount = f.universityFaculties.Count(uf => !uf.isDeleted),
                majors = f.facultyMajors
                    .Where(fm => !fm.isDeleted)
                    .OrderBy(fm => fm.major.name)
                    .Select(fm => new AdminMajorItemDto(
                        fm.major.id,
                        fm.major.name,
                        fm.major.slug,
                        fm.isActive
                    )).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (fac == null)
            return ServiceResult<AdminFacultyDetailDto>.Fail(AppMessages.ItemNotFound);

        var linkedMajorIds = fac.majors.Select(m => m.id).ToHashSet();
        var availableMajors = await _context.majors
            .AsNoTracking()
            .Where(m => !linkedMajorIds.Contains(m.id) && m.isActive)
            .OrderBy(m => m.name)
            .Select(m => new MajorDto(m.id, m.name, m.slug))
            .ToListAsync(cancellationToken);

        var result = new AdminFacultyDetailDto(
            fac.id,
            fac.name,
            fac.slug,
            fac.isActive,
            fac.universitiesCount,
            fac.majors,
            availableMajors
        );

        return ServiceResult<AdminFacultyDetailDto>.Success(result);
    }

    public async Task<ServiceResult<Guid>> CreateFacultyAsync(CreateFacultyRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.name))
            return ServiceResult<Guid>.Fail("يرجى إدخال اسم الكلية.");

        var slug = string.IsNullOrWhiteSpace(request.slug)
            ? request.name.Trim().ToLower().Replace(' ', '-')
            : request.slug.Trim().ToLower().Replace(' ', '-');

        var exists = await _context.faculties.AnyAsync(f => f.slug == slug || f.name.ToLower() == request.name.Trim().ToLower(), cancellationToken);
        if (exists)
            return ServiceResult<Guid>.Fail(AppMessages.DuplicateAcademicEntry);

        var faculty = new Faculty
        {
            name = request.name.Trim(),
            slug = slug,
            isActive = true
        };

        await _context.faculties.AddAsync(faculty, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<Guid>.Success(faculty.id, AppMessages.FacultyCreatedSuccess);
    }

    public async Task<ServiceResult<bool>> UpdateFacultyAsync(UpdateFacultyRequestDto request, CancellationToken cancellationToken = default)
    {
        var fac = await _context.faculties.FirstOrDefaultAsync(f => f.id == request.id, cancellationToken);
        if (fac == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        fac.name = request.name.Trim();
        if (!string.IsNullOrWhiteSpace(request.slug))
            fac.slug = request.slug.Trim().ToLower().Replace(' ', '-');
        fac.isActive = request.isActive;

        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(true, AppMessages.FacultyUpdatedSuccess);
    }

    public async Task<ServiceResult<bool>> ToggleFacultyStatusAsync(Guid facultyId, CancellationToken cancellationToken = default)
    {
        var fac = await _context.faculties.FirstOrDefaultAsync(f => f.id == facultyId, cancellationToken);
        if (fac == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        fac.isActive = !fac.isActive;
        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(fac.isActive, AppMessages.FacultyStatusToggled);
    }

    public async Task<ServiceResult<bool>> LinkMajorToFacultyAsync(LinkMajorToFacultyRequestDto request, CancellationToken cancellationToken = default)
    {
        var facExists = await _context.faculties.AnyAsync(f => f.id == request.facultyId, cancellationToken);
        var majExists = await _context.majors.AnyAsync(m => m.id == request.majorId, cancellationToken);
        if (!facExists || !majExists)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        var existingLink = await _context.facultyMajors
            .FirstOrDefaultAsync(fm => fm.facultyId == request.facultyId && fm.majorId == request.majorId, cancellationToken);

        if (existingLink != null)
        {
            if (existingLink.isDeleted)
            {
                existingLink.isDeleted = false;
                existingLink.isActive = true;
                await _context.SaveChangesAsync(cancellationToken);
                return ServiceResult<bool>.Success(true, AppMessages.MajorLinkedSuccess);
            }
            return ServiceResult<bool>.Fail(AppMessages.DuplicateAcademicEntry);
        }

        var link = new FacultyMajor
        {
            facultyId = request.facultyId,
            majorId = request.majorId,
            isActive = true
        };

        await _context.facultyMajors.AddAsync(link, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true, AppMessages.MajorLinkedSuccess);
    }

    public async Task<ServiceResult<bool>> UnlinkMajorFromFacultyAsync(Guid facultyId, Guid majorId, CancellationToken cancellationToken = default)
    {
        var link = await _context.facultyMajors.FirstOrDefaultAsync(fm => fm.facultyId == facultyId && fm.majorId == majorId, cancellationToken);
        if (link == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        link.isDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(true, AppMessages.MajorUnlinkedSuccess);
    }

    // ===================================================================
    // Academic Catalog: Majors Master Catalog
    // ===================================================================
    public async Task<ServiceResult<List<AdminMajorItemDto>>> GetMajorsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _context.majors
            .AsNoTracking()
            .OrderBy(m => m.name)
            .Select(m => new AdminMajorItemDto(m.id, m.name, m.slug, m.isActive))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<AdminMajorItemDto>>.Success(items);
    }

    public async Task<ServiceResult<Guid>> CreateMajorAsync(CreateMajorRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.name))
            return ServiceResult<Guid>.Fail("يرجى إدخال اسم التخصص.");

        var slug = string.IsNullOrWhiteSpace(request.slug)
            ? request.name.Trim().ToLower().Replace(' ', '-')
            : request.slug.Trim().ToLower().Replace(' ', '-');

        var exists = await _context.majors.AnyAsync(m => m.slug == slug || m.name.ToLower() == request.name.Trim().ToLower(), cancellationToken);
        if (exists)
            return ServiceResult<Guid>.Fail(AppMessages.DuplicateAcademicEntry);

        var major = new Major
        {
            name = request.name.Trim(),
            slug = slug,
            isActive = true
        };

        await _context.majors.AddAsync(major, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<Guid>.Success(major.id, AppMessages.MajorCreatedSuccess);
    }

    // Listing Moderation & Approval
    public async Task<ServiceResult<List<AdminListingItemDto>>> GetPendingApprovalListingsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _context.listings
            .AsNoTracking()
            .Where(l => l.status == ListingStatus.PendingApproval || l.status == ListingStatus.PendingConfirmation)
            .OrderByDescending(l => l.createdAt)
            .Select(l => new AdminListingItemDto(
                l.id,
                l.title,
                l.price,
                l.type,
                l.status,
                l.user.fullName,
                l.university.name,
                l.primaryFaculty != null ? l.primaryFaculty.name : null,
                l.viewsCount,
                0,
                l.createdAt
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<AdminListingItemDto>>.Success(items);
    }

    public async Task<ServiceResult<bool>> ApproveListingAsync(Guid listingId, CancellationToken cancellationToken = default)
    {
        var listing = await _context.listings.FirstOrDefaultAsync(l => l.id == listingId, cancellationToken);
        if (listing == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        listing.status = ListingStatus.Active;
        await _context.SaveChangesAsync(cancellationToken);
        await _quotaService.SyncUserActiveListingsCountAsync(listing.userId, cancellationToken);

        return ServiceResult<bool>.Success(true, "تمت الموافقة على الإعلان ونشره بنجاح.");
    }

    public async Task<ServiceResult<bool>> RejectListingAsync(Guid listingId, string reason, CancellationToken cancellationToken = default)
    {
        var listing = await _context.listings.FirstOrDefaultAsync(l => l.id == listingId, cancellationToken);
        if (listing == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        listing.status = ListingStatus.Rejected;
        await _context.SaveChangesAsync(cancellationToken);
        await _quotaService.SyncUserActiveListingsCountAsync(listing.userId, cancellationToken);

        return ServiceResult<bool>.Success(true, $"تم رفض الإعلان: {reason}");
    }

    // Roles & Permissions Management
    public async Task<ServiceResult<List<RoleDto>>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        var items = await _context.roles
            .AsNoTracking()
            .OrderBy(r => r.name)
            .Select(r => new RoleDto(
                r.id,
                r.name,
                r.normalizedName,
                r.description,
                r.isSystemRole,
                r.rolePermissions.Select(rp => rp.permission.systemName).ToList(),
                r.users.Count()
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<RoleDto>>.Success(items);
    }

    public async Task<ServiceResult<RoleDto>> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _context.roles
            .AsNoTracking()
            .Where(r => r.id == roleId)
            .Select(r => new RoleDto(
                r.id,
                r.name,
                r.normalizedName,
                r.description,
                r.isSystemRole,
                r.rolePermissions.Select(rp => rp.permission.systemName).ToList(),
                r.users.Count()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (role == null)
            return ServiceResult<RoleDto>.Fail(AppMessages.ItemNotFound);

        return ServiceResult<RoleDto>.Success(role);
    }

    public async Task<ServiceResult<Guid>> CreateRoleAsync(CreateRoleDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.name))
            return ServiceResult<Guid>.Fail("يرجى إدخال اسم الدور.");

        var normalized = dto.name.Trim().ToUpperInvariant().Replace(" ", "_");
        var exists = await _context.roles.AnyAsync(r => r.normalizedName == normalized, cancellationToken);
        if (exists)
            return ServiceResult<Guid>.Fail("يوجد دور مسجل مسبقاً بنفس الاسم.");

        var role = new Role
        {
            name = dto.name.Trim(),
            normalizedName = normalized,
            description = dto.description?.Trim(),
            isSystemRole = false
        };

        await _context.roles.AddAsync(role, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        if (dto.permissionIds != null && dto.permissionIds.Count > 0)
        {
            foreach (var pId in dto.permissionIds)
            {
                await _context.rolePermissions.AddAsync(new RolePermission
                {
                    roleId = role.id,
                    permissionId = pId
                }, cancellationToken);
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        return ServiceResult<Guid>.Success(role.id, "تم إنشاء الدور بنجاح.");
    }

    public async Task<ServiceResult<bool>> UpdateRolePermissionsAsync(UpdateRolePermissionsDto dto, CancellationToken cancellationToken = default)
    {
        var role = await _context.roles
            .Include(r => r.rolePermissions)
            .FirstOrDefaultAsync(r => r.id == dto.roleId, cancellationToken);

        if (role == null)
            return ServiceResult<bool>.Fail(AppMessages.ItemNotFound);

        // Remove obsolete permissions
        var toRemove = role.rolePermissions
            .Where(rp => !dto.permissionIds.Contains(rp.permissionId))
            .ToList();

        foreach (var item in toRemove)
        {
            _context.rolePermissions.Remove(item);
        }

        // Add new permissions
        var existingIds = role.rolePermissions.Select(rp => rp.permissionId).ToHashSet();
        var toAdd = dto.permissionIds
            .Where(id => !existingIds.Contains(id))
            .Select(id => new RolePermission
            {
                roleId = role.id,
                permissionId = id
            });

        await _context.rolePermissions.AddRangeAsync(toAdd, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true, "تم تحديث صلاحيات الدور بنجاح.");
    }

    public async Task<ServiceResult<List<PermissionDto>>> GetAllPermissionsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _context.permissions
            .AsNoTracking()
            .OrderBy(p => p.module)
            .ThenBy(p => p.displayName)
            .Select(p => new PermissionDto(
                p.id,
                p.systemName,
                p.displayName,
                p.module,
                p.description
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<PermissionDto>>.Success(items);
    }

    // Plans / Packages Management (الباقات)
    public async Task<ServiceResult<List<PlanDto>>> GetPlansAsync(CancellationToken cancellationToken = default)
    {
        var items = await _context.plans
            .AsNoTracking()
            .OrderBy(p => p.price)
            .Select(p => new PlanDto(
                p.id,
                p.name,
                p.description,
                p.price,
                p.maxActiveListings,
                p.durationDays,
                p.hasVerifiedBadge,
                p.hasStoreProfile,
                p.isDefault,
                p.isActive,
                p.users.Count()
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<PlanDto>>.Success(items);
    }

    public async Task<ServiceResult<Guid>> CreatePlanAsync(CreatePlanDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.name))
            return ServiceResult<Guid>.Fail("يرجى إدخال اسم الباقة.");

        var plan = new Plan
        {
            name = dto.name.Trim(),
            description = dto.description?.Trim(),
            price = Math.Round(Math.Max(0m, dto.price), 0, MidpointRounding.AwayFromZero),
            maxActiveListings = dto.maxActiveListings > 0 ? dto.maxActiveListings : 3,
            durationDays = dto.durationDays > 0 ? dto.durationDays : 30,
            hasVerifiedBadge = dto.hasVerifiedBadge,
            hasStoreProfile = dto.hasStoreProfile,
            isDefault = dto.isDefault,
            isActive = true
        };

        await _context.plans.AddAsync(plan, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ServiceResult<Guid>.Success(plan.id, "تم إنشاء الباقة بنجاح.");
    }
}
