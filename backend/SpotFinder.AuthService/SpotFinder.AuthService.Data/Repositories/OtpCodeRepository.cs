using Microsoft.EntityFrameworkCore;
using SpotFinder.AuthService.Data.Context;
using SpotFinder.AuthService.Data.Entities;

namespace SpotFinder.AuthService.Data.Repositories;

public class OtpCodeRepository : IOtpCodeRepository
{
    private readonly AppDbContext _context;

    public OtpCodeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OtpCode otpCode, CancellationToken ct = default)
        => await _context.OtpCodes.AddAsync(otpCode, ct);

    public async Task<OtpCode?> GetLatestActiveAsync(string phoneNumber, CancellationToken ct = default)
        => await _context.OtpCodes
            .Where(o => o.PhoneNumber == phoneNumber && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(ct);

    public Task MarkAsUsedAsync(OtpCode otpCode, CancellationToken ct = default)
    {
        otpCode.IsUsed = true;
        _context.OtpCodes.Update(otpCode);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
