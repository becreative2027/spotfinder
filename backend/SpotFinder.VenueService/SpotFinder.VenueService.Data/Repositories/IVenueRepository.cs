using SpotFinder.VenueService.Data.Entities;

namespace SpotFinder.VenueService.Data.Repositories;

public interface IVenueRepository
{
    Task<(IEnumerable<Venue> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<Venue?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Venue venue, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task DeleteAsync(Venue venue, CancellationToken ct = default);
}
