using Microsoft.EntityFrameworkCore;
using SpotFinder.VenueService.Data.Context;
using SpotFinder.VenueService.Data.Entities;

namespace SpotFinder.VenueService.Data.Repositories;

public class VenueRepository : IVenueRepository
{
    private readonly AppDbContext _db;
    public VenueRepository(AppDbContext db) => _db = db;

    public async Task<(IEnumerable<Venue> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.Venues
            .Where(v => v.IsActive)
            .Include(v => v.District)
            .Include(v => v.Photos)
            .Include(v => v.VenueConcepts).ThenInclude(vc => vc.ConceptTag)
            .OrderByDescending(v => v.AverageRating)
            .ThenByDescending(v => v.CreatedAt);

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public async Task<Venue?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Venues
            .Where(v => v.IsActive)
            .Include(v => v.District)
            .Include(v => v.Photos.OrderBy(p => p.DisplayOrder))
            .Include(v => v.VenueConcepts).ThenInclude(vc => vc.ConceptTag)
            .FirstOrDefaultAsync(v => v.Id == id, ct);

    public async Task AddAsync(Venue venue, CancellationToken ct = default) =>
        await _db.Venues.AddAsync(venue, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _db.SaveChangesAsync(ct);

    public Task DeleteAsync(Venue venue, CancellationToken ct = default)
    {
        _db.Venues.Remove(venue);
        return Task.CompletedTask;
    }
}
