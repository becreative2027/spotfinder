using SpotFinder.AuthService.Data.Entities;

namespace SpotFinder.AuthService.Business.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Guid? GetUserIdFromToken(string token);
}
