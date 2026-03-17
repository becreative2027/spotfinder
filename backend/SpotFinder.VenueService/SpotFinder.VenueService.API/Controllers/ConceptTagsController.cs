using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotFinder.VenueService.Business.Commands;
using SpotFinder.VenueService.Business.Queries;

namespace SpotFinder.VenueService.API.Controllers;

[ApiController]
[Route("api/v1/concept-tags")]
public class ConceptTagsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ConceptTagsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetConceptTagsQuery(), ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] CreateConceptTagCommand command, CancellationToken ct = default)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var deleted = await _mediator.Send(new DeleteConceptTagCommand(id), ct);
        return deleted ? NoContent() : NotFound();
    }
}
