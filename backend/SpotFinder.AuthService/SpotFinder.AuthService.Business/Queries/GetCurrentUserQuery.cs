using FluentValidation;
using MediatR;
using SpotFinder.AuthService.Business.Models;
using SpotFinder.AuthService.Data.Repositories;

namespace SpotFinder.AuthService.Business.Queries;

// 1. Query
public record GetCurrentUserQuery(Guid UserId) : IRequest<UserDto>;

// 2. Handler
public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, ct)
            ?? throw new KeyNotFoundException("Kullanıcı bulunamadı.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Hesabınız devre dışı bırakılmıştır.");

        return new UserDto(user.Id, user.Email, user.FullName, user.AvatarUrl, user.Provider, user.Role);
    }
}

// 3. Validator
public class GetCurrentUserQueryValidator : AbstractValidator<GetCurrentUserQuery>
{
    public GetCurrentUserQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Kullanıcı ID zorunludur.");
    }
}
