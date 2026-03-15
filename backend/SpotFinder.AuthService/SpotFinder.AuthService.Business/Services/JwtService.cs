using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SpotFinder.AuthService.Data.Entities;

namespace SpotFinder.AuthService.Business.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]!;
        var issuer = jwtSettings["Issuer"]!;
        var audience = jwtSettings["Audience"]!;
        var expiryMinutes = int.Parse(jwtSettings["AccessTokenExpiryMinutes"] ?? "15");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("provider", user.Provider),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public Guid? GetUserIdFromToken(string token)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]!;

            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

            return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : null;
        }
        catch
        {
            return null;
        }
    }
}
