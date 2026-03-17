using MediatR;
using SpotFinder.ReviewService.Business.Services;
using SpotFinder.ReviewService.Data.Repositories;

namespace SpotFinder.ReviewService.Business.Commands;

public record RejectReviewCommand(Guid Id) : IRequest<bool>;

public class RejectReviewCommandHandler : IRequestHandler<RejectReviewCommand, bool>
{
    private readonly IReviewRepository _repo;
    private readonly IVenueRatingService _ratingService;

    public RejectReviewCommandHandler(IReviewRepository repo, IVenueRatingService ratingService)
    {
        _repo = repo;
        _ratingService = ratingService;
    }

    public async Task<bool> Handle(RejectReviewCommand request, CancellationToken ct)
    {
        var review = await _repo.GetByIdAsync(request.Id, ct);
        if (review == null) return false;

        review.Status = "rejected";
        review.UpdatedAt = DateTime.UtcNow;
        await _repo.SaveChangesAsync(ct);

        // Recalculate in case it was previously approved
        await _ratingService.UpdateVenueRatingAsync(review.VenueId, ct);

        return true;
    }
}
