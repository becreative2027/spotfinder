using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotFinder.AuthService.Business.Commands;
using SpotFinder.AuthService.Business.Queries;
using System.Security.Claims;

namespace SpotFinder.AuthService.API.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("favorites/{venueId:guid}")]
    public async Task<IActionResult> AddFavorite(Guid venueId, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var result = await _mediator.Send(new AddFavoriteCommand(userId.Value, venueId), ct);
        return StatusCode(201, result);
    }

    [HttpDelete("favorites/{venueId:guid}")]
    public async Task<IActionResult> RemoveFavorite(Guid venueId, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        await _mediator.Send(new RemoveFavoriteCommand(userId.Value, venueId), ct);
        return NoContent();
    }

    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavorites(CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var result = await _mediator.Send(new GetFavoritesQuery(userId.Value), ct);
        return Ok(result);
    }

    [HttpPost("visits/{venueId:guid}")]
    public async Task<IActionResult> AddVisit(Guid venueId, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var result = await _mediator.Send(new AddVisitCommand(userId.Value, venueId), ct);
        return StatusCode(201, result);
    }

    [HttpGet("visits")]
    public async Task<IActionResult> GetVisits(CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var result = await _mediator.Send(new GetVisitsQuery(userId.Value), ct);
        return Ok(result);
    }

    private Guid? GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
