using MediatR;
using SpotFinder.ReviewService.Business.Services;
using SpotFinder.ReviewService.Data.Repositories;

namespace SpotFinder.ReviewService.Business.Commands;

public record ApproveReviewCommand(Guid Id) : IRequest<bool>;

public class ApproveReviewCommandHandler : IRequestHandler<ApproveReviewCommand, bool>
{
    private readonly IReviewRepository _repo;
    private readonly IVenueRatingService _ratingService;

    public ApproveReviewCommandHandler(IReviewRepository repo, IVenueRatingService ratingService)
    {
        _repo = repo;
        _ratingService = ratingService;
    }

    public async Task<bool> Handle(ApproveReviewCommand request, CancellationToken ct)
    {
        var review = await _repo.GetByIdAsync(request.Id, ct);
        if (review == null) return false;

        review.Status = "approved";
        review.UpdatedAt = DateTime.UtcNow;
        await _repo.SaveChangesAsync(ct);

        await _ratingService.UpdateVenueRatingAsync(review.VenueId, ct);

        return true;
    }
}
