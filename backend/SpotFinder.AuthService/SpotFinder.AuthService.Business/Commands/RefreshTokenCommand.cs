using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using SpotFinder.AuthService.Business.Models;
using SpotFinder.AuthService.Business.Services;
using SpotFinder.AuthService.Data.Entities;
using SpotFinder.AuthService.Data.Repositories;

namespace SpotFinder.AuthService.Business.Commands;

// 1. Command
public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResult>;

// 2. Handler
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResult>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService,
        IConfiguration configuration)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct)
            ?? throw new UnauthorizedAccessException("Geçersiz refresh token.");

        if (storedToken.IsRevoked)
            throw new UnauthorizedAccessException("Bu refresh token iptal edilmiştir.");

        if (storedToken.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token süresi dolmuştur.");

        var user = storedToken.User;

        await _refreshTokenRepository.RevokeAsync(storedToken, ct);

        var accessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshTokenValue = _jwtService.GenerateRefreshToken();
        var refreshTokenExpiry = int.Parse(
            _configuration["JwtSettings:RefreshTokenExpiryDays"] ?? "7");

        var newRefreshToken = new UserRefreshToken
        {
            UserId = user.Id,
            Token = newRefreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiry)
        };

        await _refreshTokenRepository.AddAsync(newRefreshToken, ct);
        await _refreshTokenRepository.SaveChangesAsync(ct);

        var expiryMinutes = int.Parse(
            _configuration["JwtSettings:AccessTokenExpiryMinutes"] ?? "15");

        return new AuthResult(
            accessToken,
            newRefreshTokenValue,
            DateTime.UtcNow.AddMinutes(expiryMinutes),
            new UserDto(user.Id, user.Email, user.FullName, user.AvatarUrl, user.Provider, user.Role)
        );
    }
}

// 3. Validator
public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token zorunludur.");
    }
}
