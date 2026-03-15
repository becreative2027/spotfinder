namespace SpotFinder.AuthService.Business.Services;

public record GoogleUserInfo(string Sub, string Email, string? FullName, string? AvatarUrl);

public interface IGoogleAuthService
{
    Task<GoogleUserInfo> ValidateTokenAsync(string idToken, CancellationToken ct = default);
}
