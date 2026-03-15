using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotFinder.VenueService.Business.Commands;
using SpotFinder.VenueService.Business.Queries;

namespace SpotFinder.VenueService.API.Controllers;

[ApiController]
[Route("api/v1/venues")]
public class VenuesController : ControllerBase
{
    private readonly IMediator _mediator;
    public VenuesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetVenuesQuery(page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetVenueByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] CreateVenueCommand command, CancellationToken ct = default)
    {
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVenueCommand command, CancellationToken ct = default)
    {
        if (id != command.Id) return BadRequest(new { message = "Route id ve body id eşleşmiyor." });
        var result = await _mediator.Send(command, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var deleted = await _mediator.Send(new DeleteVenueCommand(id), ct);
        return deleted ? NoContent() : NotFound();
    }
}
