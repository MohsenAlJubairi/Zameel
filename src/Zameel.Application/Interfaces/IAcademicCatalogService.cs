using Zameel.Application.Common;
using Zameel.Application.DTOs;

namespace Zameel.Application.Interfaces;

public interface IAcademicCatalogService
{
    Task<ServiceResult<List<UniversityDto>>> GetActiveUniversitiesAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult<List<FacultyDto>>> GetFacultiesByUniversityAsync(Guid universityId, CancellationToken cancellationToken = default);
    Task<ServiceResult<List<MajorDto>>> GetMajorsByFacultyAsync(Guid facultyId, CancellationToken cancellationToken = default);
}
