using Microsoft.EntityFrameworkCore;
using Zameel.Domain.Entities;

namespace Zameel.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<University> universities { get; }
    DbSet<Faculty> faculties { get; }
    DbSet<Major> majors { get; }
    DbSet<UniversityFaculty> universityFaculties { get; }
    DbSet<FacultyMajor> facultyMajors { get; }
    DbSet<Role> roles { get; }
    DbSet<Permission> permissions { get; }
    DbSet<RolePermission> rolePermissions { get; }
    DbSet<Plan> plans { get; }
    DbSet<User> users { get; }
    DbSet<StoreProfile> storeProfiles { get; }
    DbSet<Listing> listings { get; }
    DbSet<ListingImage> listingImages { get; }
    DbSet<ListingFaculty> listingFaculties { get; }
    DbSet<ListingMajor> listingMajors { get; }
    DbSet<Comment> comments { get; }
    DbSet<Review> reviews { get; }
    DbSet<ListingReport> listingReports { get; }
    DbSet<ChatMessage> chatMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
