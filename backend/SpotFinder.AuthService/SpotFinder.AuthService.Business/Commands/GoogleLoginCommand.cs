using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using SpotFinder.AuthService.Business.Models;
using SpotFinder.AuthService.Business.Services;
using SpotFinder.AuthService.Data.Entities;
using SpotFinder.AuthService.Data.Repositories;

namespace SpotFinder.AuthService.Business.Commands;

// 1. Command
public record GoogleLoginCommand(string IdToken) : IRequest<AuthResult>;

// 2. Handler
public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, AuthResult>
{
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public GoogleLoginCommandHandler(
        IGoogleAuthService googleAuthService,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService,
        IConfiguration configuration)
    {
        _googleAuthService = googleAuthService;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    public async Task<AuthResult> Handle(GoogleLoginCommand request, CancellationToken ct)
    {
        var googleUser = await _googleAuthService.ValidateTokenAsync(request.IdToken, ct);

        var user = await _userRepository.GetByEmailAsync(googleUser.Email, ct);

        if (user is null)
        {
            user = new User
            {
                Email = googleUser.Email,
                FullName = googleUser.FullName,
                AvatarUrl = googleUser.AvatarUrl,
                Provider = "google",
                Role = "user"
            };

            await _userRepository.AddAsync(user, ct);
            await _userRepository.SaveChangesAsync(ct);
        }
        else if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Hesabınız devre dışı bırakılmış.");
        }

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshTokenValue = _jwtService.GenerateRefreshToken();
        var refreshTokenExpiry = int.Parse(
            _configuration["JwtSettings:RefreshTokenExpiryDays"] ?? "7");

        var refreshToken = new UserRefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiry)
        };

        await _refreshTokenRepository.AddAsync(refreshToken, ct);
        await _refreshTokenRepository.SaveChangesAsync(ct);

        var expiryMinutes = int.Parse(
            _configuration["JwtSettings:AccessTokenExpiryMinutes"] ?? "15");

        return new AuthResult(
            accessToken,
            refreshTokenValue,
            DateTime.UtcNow.AddMinutes(expiryMinutes),
            new UserDto(user.Id, user.Email, user.FullName, user.AvatarUrl, user.Provider, user.Role)
        );
    }
}

// 3. Validator
public class GoogleLoginCommandValidator : AbstractValidator<GoogleLoginCommand>
{
    public GoogleLoginCommandValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("Google ID token zorunludur.");
    }
}
