using Zameel.Domain.Common;

namespace Zameel.Domain.Entities;

public class Faculty : BaseEntity
{
    public string name { get; set; } = string.Empty;
    public string slug { get; set; } = string.Empty;
    public bool isActive { get; set; } = true;

    // Navigation Properties
    public ICollection<UniversityFaculty> universityFaculties { get; set; } = new List<UniversityFaculty>();
    public ICollection<FacultyMajor> facultyMajors { get; set; } = new List<FacultyMajor>();
    public ICollection<User> users { get; set; } = new List<User>();
    public ICollection<ListingFaculty> listingFaculties { get; set; } = new List<ListingFaculty>();
    public ICollection<Listing> primaryListings { get; set; } = new List<Listing>();
}
