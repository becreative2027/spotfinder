using SpotFinder.AuthService.Data.Entities;

namespace SpotFinder.AuthService.Data.Repositories;

public interface IUserInteractionRepository
{
    Task<bool> FavoriteExistsAsync(Guid userId, Guid venueId, CancellationToken ct = default);
    Task AddFavoriteAsync(UserFavorite favorite, CancellationToken ct = default);
    Task<bool> RemoveFavoriteAsync(Guid userId, Guid venueId, CancellationToken ct = default);
    Task<IEnumerable<UserFavorite>> GetFavoritesAsync(Guid userId, CancellationToken ct = default);
    Task AddVisitAsync(UserVisit visit, CancellationToken ct = default);
    Task<IEnumerable<UserVisit>> GetVisitsAsync(Guid userId, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
