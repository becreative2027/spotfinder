using Microsoft.EntityFrameworkCore;
using SpotFinder.VenueService.Data.Context;
using SpotFinder.VenueService.Data.Entities;

namespace SpotFinder.VenueService.Data.Repositories;

public class DistrictRepository : IDistrictRepository
{
    private readonly AppDbContext _db;
    public DistrictRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<District>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Districts.OrderBy(d => d.Name).ToListAsync(ct);

    public async Task<District?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.Districts.FirstOrDefaultAsync(d => d.Id == id, ct);
}
