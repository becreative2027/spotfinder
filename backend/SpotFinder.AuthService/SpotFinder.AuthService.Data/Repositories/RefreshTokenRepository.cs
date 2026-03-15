using Microsoft.EntityFrameworkCore;
using SpotFinder.AuthService.Data.Context;
using SpotFinder.AuthService.Data.Entities;

namespace SpotFinder.AuthService.Data.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserRefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        => await _context.UserRefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token, ct);

    public async Task AddAsync(UserRefreshToken token, CancellationToken ct = default)
        => await _context.UserRefreshTokens.AddAsync(token, ct);

    public Task RevokeAsync(UserRefreshToken token, CancellationToken ct = default)
    {
        token.IsRevoked = true;
        _context.UserRefreshTokens.Update(token);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
