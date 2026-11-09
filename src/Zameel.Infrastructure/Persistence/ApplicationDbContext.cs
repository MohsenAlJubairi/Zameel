using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Zameel.Application.Interfaces;
using Zameel.Domain.Common;
using Zameel.Domain.Entities;

namespace Zameel.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<University> universities => Set<University>();
    public DbSet<Faculty> faculties => Set<Faculty>();
    public DbSet<Major> majors => Set<Major>();
    public DbSet<UniversityFaculty> universityFaculties => Set<UniversityFaculty>();
    public DbSet<FacultyMajor> facultyMajors => Set<FacultyMajor>();
    public DbSet<Role> roles => Set<Role>();
    public DbSet<Permission> permissions => Set<Permission>();
    public DbSet<RolePermission> rolePermissions => Set<RolePermission>();
    public DbSet<Plan> plans => Set<Plan>();
    public DbSet<User> users => Set<User>();
    public DbSet<StoreProfile> storeProfiles => Set<StoreProfile>();
    public DbSet<Listing> listings => Set<Listing>();
    public DbSet<ListingImage> listingImages => Set<ListingImage>();
    public DbSet<ListingFaculty> listingFaculties => Set<ListingFaculty>();
    public DbSet<ListingMajor> listingMajors => Set<ListingMajor>();
    public DbSet<Comment> comments => Set<Comment>();
    public DbSet<Review> reviews => Set<Review>();
    public DbSet<ListingReport> listingReports => Set<ListingReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations (Fluent API) from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Apply Global Query Filter for Soft Delete on all BaseEntity types
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(BaseEntity.isDeleted));
                var falseConstant = Expression.Constant(false);
                var lambdaComparison = Expression.Equal(property, falseConstant);
                var lambda = Expression.Lambda(lambdaComparison, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.createdAt == default)
                {
                    entry.Entity.createdAt = DateTime.UtcNow;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.updatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
