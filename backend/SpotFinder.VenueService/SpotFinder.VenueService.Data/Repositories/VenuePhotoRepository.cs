using Microsoft.EntityFrameworkCore;
using SpotFinder.VenueService.Data.Context;
using SpotFinder.VenueService.Data.Entities;

namespace SpotFinder.VenueService.Data.Repositories;

public class VenuePhotoRepository : IVenuePhotoRepository
{
    private readonly AppDbContext _db;
    public VenuePhotoRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(VenuePhoto photo, CancellationToken ct = default) =>
        await _db.VenuePhotos.AddAsync(photo, ct);

    public async Task<VenuePhoto?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.VenuePhotos.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var photo = await _db.VenuePhotos.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (photo is null) return false;
        _db.VenuePhotos.Remove(photo);
        return true;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _db.SaveChangesAsync(ct);
}
