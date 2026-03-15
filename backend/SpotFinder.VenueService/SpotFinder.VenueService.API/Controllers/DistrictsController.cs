using MediatR;
using Microsoft.AspNetCore.Mvc;
using SpotFinder.VenueService.Business.Queries;

namespace SpotFinder.VenueService.API.Controllers;

[ApiController]
[Route("api/v1/districts")]
public class DistrictsController : ControllerBase
{
    private readonly IMediator _mediator;
    public DistrictsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetDistrictsQuery(), ct);
        return Ok(result);
    }
}
