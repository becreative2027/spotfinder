using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using SpotFinder.AuthService.Business.Models;
using SpotFinder.AuthService.Business.Services;
using SpotFinder.AuthService.Data.Entities;
using SpotFinder.AuthService.Data.Repositories;

namespace SpotFinder.AuthService.Business.Commands;

// 1. Command
public record RegisterCommand(string FullName, string Email, string Password) : IRequest<AuthResult>;

// 2. Handler
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public RegisterCommandHandler(
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

    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken ct)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email, ct))
            throw new InvalidOperationException("Bu e-posta adresi zaten kullanılıyor.");

        var user = new User
        {
            Email = request.Email,
            FullName = request.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Provider = "local",
            Role = "user"
        };

        await _userRepository.AddAsync(user, ct);
        await _userRepository.SaveChangesAsync(ct);

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
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Ad Soyad zorunludur.")
            .MaximumLength(150).WithMessage("Ad Soyad en fazla 150 karakter olabilir.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta zorunludur.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
            .MaximumLength(255).WithMessage("E-posta en fazla 255 karakter olabilir.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre zorunludur.")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
            .MaximumLength(100).WithMessage("Şifre en fazla 100 karakter olabilir.");
    }
}
