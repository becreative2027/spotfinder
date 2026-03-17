using Microsoft.EntityFrameworkCore;
using SpotFinder.AuthService.Data.Context;
using SpotFinder.AuthService.Data.Entities;

namespace SpotFinder.AuthService.Data.Repositories;

public class UserInteractionRepository : IUserInteractionRepository
{
    private readonly AppDbContext _context;

    public UserInteractionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> FavoriteExistsAsync(Guid userId, Guid venueId, CancellationToken ct = default)
        => await _context.UserFavorites.AnyAsync(f => f.UserId == userId && f.VenueId == venueId, ct);

    public async Task AddFavoriteAsync(UserFavorite favorite, CancellationToken ct = default)
        => await _context.UserFavorites.AddAsync(favorite, ct);

    public async Task<bool> RemoveFavoriteAsync(Guid userId, Guid venueId, CancellationToken ct = default)
    {
        var favorite = await _context.UserFavorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.VenueId == venueId, ct);
        if (favorite is null) return false;
        _context.UserFavorites.Remove(favorite);
        return true;
    }

    public async Task<IEnumerable<UserFavorite>> GetFavoritesAsync(Guid userId, CancellationToken ct = default)
        => await _context.UserFavorites
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(ct);

    public async Task AddVisitAsync(UserVisit visit, CancellationToken ct = default)
        => await _context.UserVisits.AddAsync(visit, ct);

    public async Task<IEnumerable<UserVisit>> GetVisitsAsync(Guid userId, CancellationToken ct = default)
        => await _context.UserVisits
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.VisitedAt)
            .ToListAsync(ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
