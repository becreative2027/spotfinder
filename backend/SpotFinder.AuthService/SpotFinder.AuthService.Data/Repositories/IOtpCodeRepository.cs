using SpotFinder.AuthService.Data.Entities;

namespace SpotFinder.AuthService.Data.Repositories;

public interface IOtpCodeRepository
{
    Task AddAsync(OtpCode otpCode, CancellationToken ct = default);
    Task<OtpCode?> GetLatestActiveAsync(string phoneNumber, CancellationToken ct = default);
    Task MarkAsUsedAsync(OtpCode otpCode, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
