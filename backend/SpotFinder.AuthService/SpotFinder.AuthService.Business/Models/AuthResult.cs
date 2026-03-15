namespace SpotFinder.AuthService.Business.Models;

public record AuthResult(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiry,
    UserDto User
);

public record UserDto(
    Guid Id,
    string Email,
    string? FullName,
    string? AvatarUrl,
    string Provider,
    string Role
);
