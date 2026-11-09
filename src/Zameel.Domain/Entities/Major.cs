using Zameel.Domain.Common;

namespace Zameel.Domain.Entities;

public class Major : BaseEntity
{
    public string name { get; set; } = string.Empty;
    public string slug { get; set; } = string.Empty;
    public bool isActive { get; set; } = true;

    // Navigation Properties
    public ICollection<FacultyMajor> facultyMajors { get; set; } = new List<FacultyMajor>();
    public ICollection<User> users { get; set; } = new List<User>();
    public ICollection<Listing> primaryListings { get; set; } = new List<Listing>();
    public ICollection<ListingMajor> listingMajors { get; set; } = new List<ListingMajor>();
}
