using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace SpotFinder.AuthService.Business.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly string _clientId;

    public GoogleAuthService(IConfiguration configuration)
    {
        _clientId = configuration["GoogleAuth:ClientId"]
            ?? throw new InvalidOperationException("GoogleAuth:ClientId yapılandırması eksik.");
    }

    public async Task<GoogleUserInfo> ValidateTokenAsync(string idToken, CancellationToken ct = default)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { _clientId }
        };

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
        catch (InvalidJwtException ex)
        {
            throw new UnauthorizedAccessException($"Geçersiz Google token: {ex.Message}");
        }

        return new GoogleUserInfo(
            Sub: payload.Subject,
            Email: payload.Email,
            FullName: payload.Name,
            AvatarUrl: payload.Picture
        );
    }
}
