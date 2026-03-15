using SpotFinder.VenueService.Data.Entities;

namespace SpotFinder.VenueService.Data.Repositories;

public interface IDistrictRepository
{
    Task<IEnumerable<District>> GetAllAsync(CancellationToken ct = default);
    Task<District?> GetByIdAsync(int id, CancellationToken ct = default);
}
