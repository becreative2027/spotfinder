using MediatR;
using SpotFinder.ReviewService.Business.Models;
using SpotFinder.ReviewService.Data.Entities;
using SpotFinder.ReviewService.Data.Repositories;

namespace SpotFinder.ReviewService.Business.Queries;

public record GetVenueReviewsQuery(Guid VenueId) : IRequest<IEnumerable<ReviewDto>>;

public class GetVenueReviewsQueryHandler : IRequestHandler<GetVenueReviewsQuery, IEnumerable<ReviewDto>>
{
    private readonly IReviewRepository _repo;

    public GetVenueReviewsQueryHandler(IReviewRepository repo) => _repo = repo;

    public async Task<IEnumerable<ReviewDto>> Handle(GetVenueReviewsQuery request, CancellationToken ct)
    {
        var reviews = await _repo.GetApprovedByVenueAsync(request.VenueId, ct);
        return reviews.Select(MapToDto);
    }

    private static ReviewDto MapToDto(Review r) =>
        new(r.Id, r.VenueId, r.UserId, r.Body, r.Rating, r.Status, r.CreatedAt, r.UpdatedAt);
}
