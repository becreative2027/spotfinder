namespace SpotFinder.ReviewService.Business.Services;

public interface IVenueRatingService
{
    Task UpdateVenueRatingAsync(Guid venueId, CancellationToken ct = default);
}
