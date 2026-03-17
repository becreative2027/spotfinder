using MediatR;
using SpotFinder.AuthService.Data.Repositories;

namespace SpotFinder.AuthService.Business.Commands;

public record RemoveFavoriteCommand(Guid UserId, Guid VenueId) : IRequest<Unit>;

public class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand, Unit>
{
    private readonly IUserInteractionRepository _repo;

    public RemoveFavoriteCommandHandler(IUserInteractionRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(RemoveFavoriteCommand request, CancellationToken ct)
    {
        var removed = await _repo.RemoveFavoriteAsync(request.UserId, request.VenueId, ct);
        if (!removed)
            throw new KeyNotFoundException("Favori bulunamadı.");

        await _repo.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
