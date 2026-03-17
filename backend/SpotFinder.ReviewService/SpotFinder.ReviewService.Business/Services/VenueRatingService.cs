using Microsoft.EntityFrameworkCore;
using SpotFinder.ReviewService.Data.Context;
using SpotFinder.ReviewService.Data.Repositories;

namespace SpotFinder.ReviewService.Business.Services;

public class VenueRatingService : IVenueRatingService
{
    private readonly IReviewRepository _reviewRepo;
    private readonly AppDbContext _db;

    public VenueRatingService(IReviewRepository reviewRepo, AppDbContext db)
    {
        _reviewRepo = reviewRepo;
        _db = db;
    }

    public async Task UpdateVenueRatingAsync(Guid venueId, CancellationToken ct = default)
    {
        var (avgRating, count) = await _reviewRepo.GetApprovedStatsAsync(venueId, ct);

        var venue = await _db.VenueRatings.FindAsync([venueId], ct);
        if (venue == null) return;

        venue.AverageRating = avgRating;
        venue.ReviewCount = count;

        await _db.SaveChangesAsync(ct);
    }
}
