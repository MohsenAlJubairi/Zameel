using Microsoft.EntityFrameworkCore;
using Zameel.Application.Common;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;

namespace Zameel.Application.Services;

public class AcademicCatalogService : IAcademicCatalogService
{
    private readonly IApplicationDbContext _context;

    public AcademicCatalogService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<List<UniversityDto>>> GetActiveUniversitiesAsync(CancellationToken cancellationToken = default)
    {
        var universities = await _context.universities
            .AsNoTracking()
            .Where(u => u.isActive)
            .OrderBy(u => u.name)
            .Select(u => new UniversityDto(
                u.id,
                u.name,
                u.city,
                u.logoUrl
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<UniversityDto>>.Success(universities);
    }

    public async Task<ServiceResult<List<FacultyDto>>> GetFacultiesByUniversityAsync(Guid universityId, CancellationToken cancellationToken = default)
    {
        if (universityId == Guid.Empty)
        {
            return ServiceResult<List<FacultyDto>>.Fail(AppMessages.ItemNotFound);
        }

        var faculties = await _context.universityFaculties
            .AsNoTracking()
            .Where(uf => uf.universityId == universityId && uf.isActive && uf.faculty.isActive)
            .OrderBy(uf => uf.customName ?? uf.faculty.name)
            .Select(uf => new FacultyDto(
                uf.faculty.id,
                uf.faculty.name,
                uf.faculty.slug,
                uf.customName ?? uf.faculty.name
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<FacultyDto>>.Success(faculties);
    }

    public async Task<ServiceResult<List<MajorDto>>> GetMajorsByFacultyAsync(Guid facultyId, CancellationToken cancellationToken = default)
    {
        if (facultyId == Guid.Empty)
        {
            return ServiceResult<List<MajorDto>>.Fail(AppMessages.ItemNotFound);
        }

        var majors = await _context.facultyMajors
            .AsNoTracking()
            .Where(fm => fm.facultyId == facultyId && fm.isActive && fm.major.isActive)
            .OrderBy(fm => fm.major.name)
            .Select(fm => new MajorDto(
                fm.major.id,
                fm.major.name,
                fm.major.slug
            ))
            .ToListAsync(cancellationToken);

        return ServiceResult<List<MajorDto>>.Success(majors);
    }
}
