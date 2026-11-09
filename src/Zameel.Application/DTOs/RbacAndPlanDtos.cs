namespace Zameel.Application.DTOs;

public record RoleDto(
    Guid id,
    string name,
    string normalizedName,
    string? description,
    bool isSystemRole,
    List<string> permissions,
    int usersCount
);

public record PermissionDto(
    Guid id,
    string systemName,
    string displayName,
    string module,
    string? description
);

public record PlanDto(
    Guid id,
    string name,
    string? description,
    decimal price,
    int maxActiveListings,
    int durationDays,
    bool hasVerifiedBadge,
    bool hasStoreProfile,
    bool isDefault,
    bool isActive,
    int subscribersCount
);

public record CreateRoleDto(
    string name,
    string? description,
    List<Guid> permissionIds
);

public record UpdateRolePermissionsDto(
    Guid roleId,
    List<Guid> permissionIds
);

public record CreatePlanDto(
    string name,
    string? description,
    decimal price,
    int maxActiveListings,
    int durationDays,
    bool hasVerifiedBadge,
    bool hasStoreProfile,
    bool isDefault
);
