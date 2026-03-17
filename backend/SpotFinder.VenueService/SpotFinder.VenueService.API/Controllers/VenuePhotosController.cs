using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotFinder.VenueService.Business.Commands;

namespace SpotFinder.VenueService.API.Controllers;

[ApiController]
[Route("api/v1/venues/{venueId:guid}/photos")]
[Authorize(Roles = "admin")]
public class VenuePhotosController : ControllerBase
{
    private readonly IMediator _mediator;
    public VenuePhotosController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> AddPhoto(Guid venueId, [FromBody] AddPhotoRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new AddVenuePhotoCommand(venueId, request.Url, request.IsMenuPhoto, request.DisplayOrder), ct);
        return StatusCode(201, result);
    }

    [HttpDelete("{photoId:guid}")]
    public async Task<IActionResult> DeletePhoto(Guid venueId, Guid photoId, CancellationToken ct)
    {
        var deleted = await _mediator.Send(new DeleteVenuePhotoCommand(venueId, photoId), ct);
        return deleted ? NoContent() : NotFound();
    }
}

public record AddPhotoRequest(string Url, bool IsMenuPhoto = false, int DisplayOrder = 0);
