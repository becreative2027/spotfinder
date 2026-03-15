namespace SpotFinder.AuthService.Business.Services;

public record AppleUserInfo(string Sub, string? Email);

public interface IAppleAuthService
{
    Task<AppleUserInfo> ValidateTokenAsync(string identityToken, CancellationToken ct = default);
}
