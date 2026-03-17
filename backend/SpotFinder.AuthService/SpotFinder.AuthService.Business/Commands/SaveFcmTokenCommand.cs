using MediatR;
using SpotFinder.AuthService.Data.Repositories;

namespace SpotFinder.AuthService.Business.Commands;

public record SaveFcmTokenCommand(Guid UserId, string FcmToken) : IRequest;

public class SaveFcmTokenCommandHandler : IRequestHandler<SaveFcmTokenCommand>
{
    private readonly IUserRepository _userRepo;

    public SaveFcmTokenCommandHandler(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task Handle(SaveFcmTokenCommand request, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(request.UserId, ct)
            ?? throw new KeyNotFoundException("Kullanıcı bulunamadı.");

        user.FcmToken = request.FcmToken;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepo.UpdateAsync(user, ct);
        await _userRepo.SaveChangesAsync(ct);
    }
}
