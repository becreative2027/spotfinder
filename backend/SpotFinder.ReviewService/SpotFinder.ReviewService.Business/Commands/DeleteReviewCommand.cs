using MediatR;
using SpotFinder.ReviewService.Business.Services;
using SpotFinder.ReviewService.Data.Context;
using SpotFinder.ReviewService.Data.Repositories;

namespace SpotFinder.ReviewService.Business.Commands;

public record DeleteReviewCommand(Guid Id) : IRequest<bool>;

public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, bool>
{
    private readonly IReviewRepository _repo;
    private readonly AppDbContext _db;
    private readonly IVenueRatingService _ratingService;

    public DeleteReviewCommandHandler(IReviewRepository repo, AppDbContext db, IVenueRatingService ratingService)
    {
        _repo = repo;
        _db = db;
        _ratingService = ratingService;
    }

    public async Task<bool> Handle(DeleteReviewCommand request, CancellationToken ct)
    {
        var review = await _repo.GetByIdAsync(request.Id, ct);
        if (review == null) return false;

        var venueId = review.VenueId;
        var wasApproved = review.Status == "approved";

        _db.Reviews.Remove(review);
        await _db.SaveChangesAsync(ct);

        if (wasApproved)
            await _ratingService.UpdateVenueRatingAsync(venueId, ct);

        return true;
    }
}
