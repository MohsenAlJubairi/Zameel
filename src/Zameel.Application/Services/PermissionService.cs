using Microsoft.EntityFrameworkCore;
using Zameel.Application.Interfaces;
using Zameel.Domain.Constants;

namespace Zameel.Application.Services;

public class PermissionService : IPermissionService
{
    private readonly IApplicationDbContext _context;

    public PermissionService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permissionName, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty || string.IsNullOrWhiteSpace(permissionName))
            return false;

        var userRole = await _context.users
            .AsNoTracking()
            .Where(u => u.id == userId)
            .Select(u => new
            {
                RoleNormalized = u.role.normalizedName,
                HasPermission = u.role.rolePermissions.Any(rp => rp.permission.systemName == permissionName)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (userRole == null)
            return false;

        // Admin has super-user access to all permissions
        if (userRole.RoleNormalized == SystemRoles.Admin)
            return true;

        return userRole.HasPermission;
    }

    public async Task<List<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            return new List<string>();

        var userRole = await _context.users
            .AsNoTracking()
            .Where(u => u.id == userId)
            .Select(u => new
            {
                RoleNormalized = u.role.normalizedName,
                RoleId = u.roleId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (userRole == null)
            return new List<string>();

        if (userRole.RoleNormalized == SystemRoles.Admin)
        {
            return await _context.permissions
                .AsNoTracking()
                .Select(p => p.systemName)
                .ToListAsync(cancellationToken);
        }

        return await _context.rolePermissions
            .AsNoTracking()
            .Where(rp => rp.roleId == userRole.RoleId)
            .Select(rp => rp.permission.systemName)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
