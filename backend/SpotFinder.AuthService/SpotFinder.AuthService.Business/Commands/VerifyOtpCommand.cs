using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using SpotFinder.AuthService.Business.Models;
using SpotFinder.AuthService.Business.Services;
using SpotFinder.AuthService.Data.Entities;
using SpotFinder.AuthService.Data.Repositories;

namespace SpotFinder.AuthService.Business.Commands;

// 1. Command
public record VerifyOtpCommand(string PhoneNumber, string Code) : IRequest<AuthResult>;

// 2. Handler
public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, AuthResult>
{
    private readonly IOtpCodeRepository _otpCodeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public VerifyOtpCommandHandler(
        IOtpCodeRepository otpCodeRepository,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService,
        IConfiguration configuration)
    {
        _otpCodeRepository = otpCodeRepository;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    public async Task<AuthResult> Handle(VerifyOtpCommand request, CancellationToken ct)
    {
        var otpCode = await _otpCodeRepository.GetLatestActiveAsync(request.PhoneNumber, ct);

        if (otpCode is null || otpCode.Code != request.Code)
            throw new UnauthorizedAccessException("Geçersiz veya süresi dolmuş OTP kodu.");

        await _otpCodeRepository.MarkAsUsedAsync(otpCode, ct);
        await _otpCodeRepository.SaveChangesAsync(ct);

        var user = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber, ct);

        if (user is null)
        {
            // Yeni kullanıcı — telefon numarası ile kayıt
            user = new User
            {
                // E-posta telefon numarasından türetilir; kullanıcı daha sonra güncelleyebilir
                Email = $"phone_{request.PhoneNumber.TrimStart('+').Replace(" ", "")}@spotfinder.local",
                PhoneNumber = request.PhoneNumber,
                Provider = "phone",
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
public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası zorunludur.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("OTP kodu zorunludur.")
            .Length(6).WithMessage("OTP kodu 6 haneli olmalıdır.")
            .Matches(@"^\d{6}$").WithMessage("OTP kodu yalnızca rakamlardan oluşmalıdır.");
    }
}
