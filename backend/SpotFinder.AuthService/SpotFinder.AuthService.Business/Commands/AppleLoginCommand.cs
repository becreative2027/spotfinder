using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using SpotFinder.AuthService.Business.Models;
using SpotFinder.AuthService.Business.Services;
using SpotFinder.AuthService.Data.Entities;
using SpotFinder.AuthService.Data.Repositories;

namespace SpotFinder.AuthService.Business.Commands;

// 1. Command
public record AppleLoginCommand(string IdentityToken, string? FullName) : IRequest<AuthResult>;

// 2. Handler
public class AppleLoginCommandHandler : IRequestHandler<AppleLoginCommand, AuthResult>
{
    private readonly IAppleAuthService _appleAuthService;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public AppleLoginCommandHandler(
        IAppleAuthService appleAuthService,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService,
        IConfiguration configuration)
    {
        _appleAuthService = appleAuthService;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    public async Task<AuthResult> Handle(AppleLoginCommand request, CancellationToken ct)
    {
        var appleUser = await _appleAuthService.ValidateTokenAsync(request.IdentityToken, ct);

        // Apple yalnızca ilk girişte e-posta döndürür; sonraki girişlerde null gelebilir.
        if (string.IsNullOrEmpty(appleUser.Email))
            throw new UnauthorizedAccessException("Apple token'ından e-posta bilgisi alınamadı.");

        var user = await _userRepository.GetByEmailAsync(appleUser.Email, ct);

        if (user is null)
        {
            user = new User
            {
                Email = appleUser.Email,
                FullName = request.FullName,
                Provider = "apple",
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
public class AppleLoginCommandValidator : AbstractValidator<AppleLoginCommand>
{
    public AppleLoginCommandValidator()
    {
        RuleFor(x => x.IdentityToken)
            .NotEmpty().WithMessage("Apple identity token zorunludur.");
    }
}
