using MediatR;
using SpotFinder.ReviewService.Business.Models;
using SpotFinder.ReviewService.Data.Entities;
using SpotFinder.ReviewService.Data.Repositories;

namespace SpotFinder.ReviewService.Business.Queries;

public record GetPendingReviewsQuery : IRequest<IEnumerable<ReviewDto>>;

public class GetPendingReviewsQueryHandler : IRequestHandler<GetPendingReviewsQuery, IEnumerable<ReviewDto>>
{
    private readonly IReviewRepository _repo;

    public GetPendingReviewsQueryHandler(IReviewRepository repo) => _repo = repo;

    public async Task<IEnumerable<ReviewDto>> Handle(GetPendingReviewsQuery request, CancellationToken ct)
    {
        var reviews = await _repo.GetPendingAsync(ct);
        return reviews.Select(r => new ReviewDto(r.Id, r.VenueId, r.UserId, r.Body, r.Rating, r.Status, r.CreatedAt, r.UpdatedAt));
    }
}
