using Zameel.Domain.Common;

namespace Zameel.Domain.Entities;

public class Permission : BaseEntity
{
    public string systemName { get; set; } = string.Empty;
    public string displayName { get; set; } = string.Empty;
    public string module { get; set; } = string.Empty;
    public string? description { get; set; }

    // Navigation Properties
    public ICollection<RolePermission> rolePermissions { get; set; } = new List<RolePermission>();
}
