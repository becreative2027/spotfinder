using Microsoft.EntityFrameworkCore;
using SpotFinder.ReviewService.Data.Context;
using SpotFinder.ReviewService.Data.Entities;

namespace SpotFinder.ReviewService.Data.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _db;

    public ReviewRepository(AppDbContext db) => _db = db;

    public async Task<Review?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Reviews.FindAsync([id], ct);

    public async Task<IEnumerable<Review>> GetApprovedByVenueAsync(Guid venueId, CancellationToken ct = default) =>
        await _db.Reviews
            .Where(r => r.VenueId == venueId && r.Status == "approved")
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<Review>> GetPendingAsync(CancellationToken ct = default) =>
        await _db.Reviews
            .Where(r => r.Status == "pending")
            .OrderBy(r => r.CreatedAt)
            .ToListAsync(ct);

    public async Task<(decimal AvgRating, int Count)> GetApprovedStatsAsync(Guid venueId, CancellationToken ct = default)
    {
        var approved = await _db.Reviews
            .Where(r => r.VenueId == venueId && r.Status == "approved")
            .Select(r => (int)r.Rating)
            .ToListAsync(ct);

        if (approved.Count == 0)
            return (0m, 0);

        var avg = Math.Round((decimal)approved.Average(), 2);
        return (avg, approved.Count);
    }

    public async Task AddAsync(Review review, CancellationToken ct = default) =>
        await _db.Reviews.AddAsync(review, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _db.SaveChangesAsync(ct);
}
