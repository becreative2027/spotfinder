using FluentValidation;
using MediatR;
using SpotFinder.AuthService.Business.Services;
using SpotFinder.AuthService.Data.Entities;
using SpotFinder.AuthService.Data.Repositories;

namespace SpotFinder.AuthService.Business.Commands;

// 1. Command
public record SendOtpCommand(string PhoneNumber) : IRequest<Unit>;

// 2. Handler
public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, Unit>
{
    private const int OtpExpiryMinutes = 5;

    private readonly IOtpCodeRepository _otpCodeRepository;
    private readonly ISmsService _smsService;

    public SendOtpCommandHandler(IOtpCodeRepository otpCodeRepository, ISmsService smsService)
    {
        _otpCodeRepository = otpCodeRepository;
        _smsService = smsService;
    }

    public async Task<Unit> Handle(SendOtpCommand request, CancellationToken ct)
    {
        var code = GenerateOtpCode();

        var otpCode = new OtpCode
        {
            PhoneNumber = request.PhoneNumber,
            Code = code,
            ExpiresAt = DateTime.UtcNow.AddMinutes(OtpExpiryMinutes)
        };

        await _otpCodeRepository.AddAsync(otpCode, ct);
        await _otpCodeRepository.SaveChangesAsync(ct);

        await _smsService.SendOtpAsync(request.PhoneNumber, code, ct);

        return Unit.Value;
    }

    private static string GenerateOtpCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}

// 3. Validator
public class SendOtpCommandValidator : AbstractValidator<SendOtpCommand>
{
    public SendOtpCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası zorunludur.")
            .Matches(@"^\+?[1-9]\d{9,14}$").WithMessage("Geçerli bir telefon numarası giriniz (örn. +905551234567).");
    }
}
