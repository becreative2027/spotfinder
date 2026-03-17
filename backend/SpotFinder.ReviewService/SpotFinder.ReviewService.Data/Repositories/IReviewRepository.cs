using SpotFinder.ReviewService.Data.Entities;

namespace SpotFinder.ReviewService.Data.Repositories;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Review>> GetApprovedByVenueAsync(Guid venueId, CancellationToken ct = default);
    Task<IEnumerable<Review>> GetPendingAsync(CancellationToken ct = default);
    Task<(decimal AvgRating, int Count)> GetApprovedStatsAsync(Guid venueId, CancellationToken ct = default);
    Task AddAsync(Review review, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
