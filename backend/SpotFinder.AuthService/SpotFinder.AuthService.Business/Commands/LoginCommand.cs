using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using SpotFinder.AuthService.Business.Models;
using SpotFinder.AuthService.Business.Services;
using SpotFinder.AuthService.Data.Entities;
using SpotFinder.AuthService.Data.Repositories;

namespace SpotFinder.AuthService.Business.Commands;

// 1. Command
public record LoginCommand(string Email, string Password) : IRequest<AuthResult>;

// 2. Handler
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    public async Task<AuthResult> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, ct)
            ?? throw new UnauthorizedAccessException("E-posta veya şifre hatalı.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Hesabınız devre dışı bırakılmıştır.");

        if (user.Provider != "local" || user.PasswordHash == null)
            throw new UnauthorizedAccessException("Bu hesap sosyal giriş ile kayıt edilmiştir. Lütfen ilgili sağlayıcı ile giriş yapınız.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("E-posta veya şifre hatalı.");

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
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta zorunludur.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre zorunludur.");
    }
}
