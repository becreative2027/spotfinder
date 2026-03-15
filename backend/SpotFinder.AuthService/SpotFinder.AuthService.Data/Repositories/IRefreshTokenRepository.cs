using SpotFinder.AuthService.Data.Entities;

namespace SpotFinder.AuthService.Data.Repositories;

public interface IRefreshTokenRepository
{
    Task<UserRefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task AddAsync(UserRefreshToken token, CancellationToken ct = default);
    Task RevokeAsync(UserRefreshToken token, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
