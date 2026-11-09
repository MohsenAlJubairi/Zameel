using Zameel.Domain.Common;

namespace Zameel.Domain.Entities;

public class Role : BaseEntity
{
    public string name { get; set; } = string.Empty;
    public string normalizedName { get; set; } = string.Empty;
    public string? description { get; set; }
    public bool isSystemRole { get; set; } = false;

    // Navigation Properties
    public ICollection<User> users { get; set; } = new List<User>();
    public ICollection<RolePermission> rolePermissions { get; set; } = new List<RolePermission>();
}
