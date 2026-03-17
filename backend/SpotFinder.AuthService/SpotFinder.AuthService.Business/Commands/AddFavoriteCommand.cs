using MediatR;
using SpotFinder.AuthService.Business.Models;
using SpotFinder.AuthService.Data.Entities;
using SpotFinder.AuthService.Data.Repositories;

namespace SpotFinder.AuthService.Business.Commands;

public record AddFavoriteCommand(Guid UserId, Guid VenueId) : IRequest<FavoriteDto>;

public class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand, FavoriteDto>
{
    private readonly IUserInteractionRepository _repo;

    public AddFavoriteCommandHandler(IUserInteractionRepository repo)
    {
        _repo = repo;
    }

    public async Task<FavoriteDto> Handle(AddFavoriteCommand request, CancellationToken ct)
    {
        var exists = await _repo.FavoriteExistsAsync(request.UserId, request.VenueId, ct);
        if (exists)
            throw new InvalidOperationException("Bu mekân zaten favorilerinizde.");

        var favorite = new UserFavorite
        {
            UserId = request.UserId,
            VenueId = request.VenueId,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddFavoriteAsync(favorite, ct);
        await _repo.SaveChangesAsync(ct);

        return new FavoriteDto(favorite.VenueId, favorite.CreatedAt);
    }
}
