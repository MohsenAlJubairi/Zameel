using Zameel.Domain.Common;

namespace Zameel.Domain.Entities;

public class RolePermission : BaseEntity
{
    public Guid roleId { get; set; }
    public Role role { get; set; } = null!;

    public Guid permissionId { get; set; }
    public Permission permission { get; set; } = null!;
}
