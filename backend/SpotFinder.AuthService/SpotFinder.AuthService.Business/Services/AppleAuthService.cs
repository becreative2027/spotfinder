using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace SpotFinder.AuthService.Business.Services;

public class AppleAuthService : IAppleAuthService
{
    private const string AppleJwksUrl = "https://appleid.apple.com/auth/keys";
    private const string AppleIssuer = "https://appleid.apple.com";

    private readonly string _clientId;
    private readonly HttpClient _httpClient;

    public AppleAuthService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _clientId = configuration["AppleAuth:ClientId"]
            ?? throw new InvalidOperationException("AppleAuth:ClientId yapılandırması eksik.");
        _httpClient = httpClientFactory.CreateClient("AppleAuth");
    }

    public async Task<AppleUserInfo> ValidateTokenAsync(string identityToken, CancellationToken ct = default)
    {
        var jwks = await FetchApplePublicKeysAsync(ct);

        var handler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = AppleIssuer,
            ValidAudience = _clientId,
            IssuerSigningKeys = jwks,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        try
        {
            var principal = handler.ValidateToken(identityToken, validationParameters, out _);

            var sub = principal.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException("Apple token'ında 'sub' claim bulunamadı.");

            var email = principal.FindFirst("email")?.Value;

            return new AppleUserInfo(Sub: sub, Email: email);
        }
        catch (SecurityTokenException ex)
        {
            throw new UnauthorizedAccessException($"Geçersiz Apple token: {ex.Message}");
        }
    }

    private async Task<IEnumerable<JsonWebKey>> FetchApplePublicKeysAsync(CancellationToken ct)
    {
        var response = await _httpClient.GetStringAsync(AppleJwksUrl, ct);
        var jwksDocument = JsonDocument.Parse(response);

        var keySet = new JsonWebKeySet(response);
        return keySet.Keys;
    }
}
