using Microsoft.AspNetCore.Mvc;
using Zameel.Application.Interfaces;

namespace Zameel.Web.Controllers;

[Route("academic-catalog")]
public class AcademicCatalogController : Controller
{
    private readonly IAcademicCatalogService _catalogService;
    private readonly IStoreService _storeService;

    public AcademicCatalogController(IAcademicCatalogService catalogService, IStoreService storeService)
    {
        _catalogService = catalogService;
        _storeService = storeService;
    }

    [HttpGet("universities")]
    public async Task<IActionResult> GetUniversities(CancellationToken cancellationToken)
    {
        var result = await _catalogService.GetActiveUniversitiesAsync(cancellationToken);
        return Json(result);
    }

    [HttpGet("faculties")]
    public async Task<IActionResult> GetFaculties([FromQuery] Guid universityId, CancellationToken cancellationToken)
    {
        var result = await _catalogService.GetFacultiesByUniversityAsync(universityId, cancellationToken);
        return Json(result);
    }

    [HttpGet("majors")]
    public async Task<IActionResult> GetMajors([FromQuery] Guid facultyId, CancellationToken cancellationToken)
    {
        var result = await _catalogService.GetMajorsByFacultyAsync(facultyId, cancellationToken);
        return Json(result);
    }

    [HttpGet("stores")]
    public async Task<IActionResult> GetStores([FromQuery] Guid universityId, CancellationToken cancellationToken)
    {
        var result = await _storeService.GetApprovedStoresByUniversityAsync(universityId, cancellationToken);
        return Json(result);
    }
}
