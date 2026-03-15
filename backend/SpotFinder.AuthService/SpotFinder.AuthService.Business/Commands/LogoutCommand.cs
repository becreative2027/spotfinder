using FluentValidation;
using MediatR;
using SpotFinder.AuthService.Data.Repositories;

namespace SpotFinder.AuthService.Business.Commands;

// 1. Command
public record LogoutCommand(string RefreshToken) : IRequest<Unit>;

// 2. Handler
public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken ct)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);

        if (storedToken != null && !storedToken.IsRevoked)
        {
            await _refreshTokenRepository.RevokeAsync(storedToken, ct);
            await _refreshTokenRepository.SaveChangesAsync(ct);
        }

        return Unit.Value;
    }
}

// 3. Validator
public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token zorunludur.");
    }
}
