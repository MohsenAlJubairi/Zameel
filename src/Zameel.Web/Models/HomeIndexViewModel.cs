using Zameel.Application.DTOs;

namespace Zameel.Web.Models;

public record HomeIndexViewModel(
    List<UniversityDto> universities,
    PagedResult<ListingCardDto> listings,
    ListingFilterDto activeFilter
);
