using Zameel.Domain.Common;

namespace Zameel.Domain.Entities;

public class University : BaseEntity
{
    public string name { get; set; } = string.Empty;
    public string city { get; set; } = string.Empty;
    public string? logoUrl { get; set; }
    public bool isActive { get; set; } = true;

    // Navigation Properties
    public ICollection<UniversityFaculty> universityFaculties { get; set; } = new List<UniversityFaculty>();
    public ICollection<User> users { get; set; } = new List<User>();
    public ICollection<Listing> listings { get; set; } = new List<Listing>();
}
