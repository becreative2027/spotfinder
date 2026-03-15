using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotFinder.SearchService.Business.Queries;

namespace SpotFinder.SearchService.API.Controllers;

[ApiController]
[Route("api/v1/search")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;
    public SearchController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Filtreli mekân arama. districtId, conceptTagIds (virgülle ayrılmış), sortBy (rating|newest), page, pageSize
    /// </summary>
    [HttpGet("venues")]
    public async Task<IActionResult> Search(
        [FromQuery] int? districtId,
        [FromQuery] string? conceptTagIds,
        [FromQuery] string sortBy = "rating",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        IEnumerable<int>? tagIds = null;
        if (!string.IsNullOrWhiteSpace(conceptTagIds))
        {
            tagIds = conceptTagIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(s => int.TryParse(s, out var id) ? id : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value);
        }

        var result = await _mediator.Send(
            new SearchVenuesQuery(districtId, tagIds, sortBy, page, pageSize), ct);

        return Ok(result);
    }

    /// <summary>
    /// Öne çıkan mekânlar (anasayfa). count parametresi ile kaç tane istediğinizi belirtin (max 50).
    /// </summary>
    [HttpGet("venues/featured")]
    public async Task<IActionResult> Featured(
        [FromQuery] int count = 10,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetFeaturedVenuesQuery(count), ct);
        return Ok(result);
    }
}
