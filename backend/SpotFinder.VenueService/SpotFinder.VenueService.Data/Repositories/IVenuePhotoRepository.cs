using SpotFinder.VenueService.Data.Entities;

namespace SpotFinder.VenueService.Data.Repositories;

public interface IVenuePhotoRepository
{
    Task AddAsync(VenuePhoto photo, CancellationToken ct = default);
    Task<VenuePhoto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
