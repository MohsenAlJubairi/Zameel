using Zameel.Domain.Common;

namespace Zameel.Domain.Entities;

public class FacultyMajor : BaseEntity
{
    public Guid facultyId { get; set; }
    public Faculty faculty { get; set; } = null!;

    public Guid majorId { get; set; }
    public Major major { get; set; } = null!;

    public bool isActive { get; set; } = true;
}
