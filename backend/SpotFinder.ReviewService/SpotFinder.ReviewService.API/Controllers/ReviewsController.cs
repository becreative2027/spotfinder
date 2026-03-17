using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpotFinder.ReviewService.Business.Commands;
using SpotFinder.ReviewService.Business.Queries;

namespace SpotFinder.ReviewService.API.Controllers;

[ApiController]
[Route("api/v1/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ReviewsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Yorum ekle (JWT gerekli)</summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var result = await _mediator.Send(
            new CreateReviewCommand(request.VenueId, Guid.Parse(userId), request.Body, request.Rating), ct);

        return CreatedAtAction(nameof(GetByVenue), new { venueId = result.VenueId }, result);
    }

    /// <summary>Mekânın onaylı yorumları</summary>
    [HttpGet("venue/{venueId:guid}")]
    public async Task<IActionResult> GetByVenue(Guid venueId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetVenueReviewsQuery(venueId), ct);
        return Ok(result);
    }

    /// <summary>Bekleyen yorumlar (admin)</summary>
    [HttpGet("pending")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetPending(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPendingReviewsQuery(), ct);
        return Ok(result);
    }

    /// <summary>Yorumu onayla (admin)</summary>
    [HttpPut("{id:guid}/approve")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken ct)
    {
        var success = await _mediator.Send(new ApproveReviewCommand(id), ct);
        return success ? NoContent() : NotFound();
    }

    /// <summary>Yorumu reddet (admin)</summary>
    [HttpPut("{id:guid}/reject")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Reject(Guid id, CancellationToken ct)
    {
        var success = await _mediator.Send(new RejectReviewCommand(id), ct);
        return success ? NoContent() : NotFound();
    }

    /// <summary>Yorum sil (admin)</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var success = await _mediator.Send(new DeleteReviewCommand(id), ct);
        return success ? NoContent() : NotFound();
    }
}

public record CreateReviewRequest(Guid VenueId, string? Body, short Rating);
