using Zameel.Domain.Common;

namespace Zameel.Domain.Entities;

public class UniversityFaculty : BaseEntity
{
    public Guid universityId { get; set; }
    public University university { get; set; } = null!;

    public Guid facultyId { get; set; }
    public Faculty faculty { get; set; } = null!;

    public string? customName { get; set; }
    public bool isActive { get; set; } = true;
}
