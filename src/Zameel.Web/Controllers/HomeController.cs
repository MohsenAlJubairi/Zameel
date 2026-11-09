using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Zameel.Application.DTOs;
using Zameel.Application.Interfaces;
using Zameel.Domain.Enums;
using Zameel.Web.Models;

namespace Zameel.Web.Controllers;

public class HomeController : Controller
{
    private readonly IListingService _listingService;
    private readonly IAcademicCatalogService _catalogService;

    public HomeController(IListingService listingService, IAcademicCatalogService catalogService)
    {
        _listingService = listingService;
        _catalogService = catalogService;
    }

    public async Task<IActionResult> Index([FromQuery] ListingFilterDto? filter, CancellationToken cancellationToken)
    {
        var activeFilter = filter ?? new ListingFilterDto();

        var listingsResult = await _listingService.GetFeedAsync(activeFilter, cancellationToken);
        var universitiesResult = await _catalogService.GetActiveUniversitiesAsync(cancellationToken);

        var viewModel = new HomeIndexViewModel(
            universitiesResult.data ?? new List<UniversityDto>(),
            listingsResult.data ?? new PagedResult<ListingCardDto>(new List<ListingCardDto>(), 0, 1, 12),
            activeFilter
        );

        return View(viewModel);
    }

    public async Task<IActionResult> Requests([FromQuery] ListingFilterDto? filter, CancellationToken cancellationToken)
    {
        var activeFilter = (filter ?? new ListingFilterDto()) with { type = ListingType.Request };
        return await Index(activeFilter, cancellationToken);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
