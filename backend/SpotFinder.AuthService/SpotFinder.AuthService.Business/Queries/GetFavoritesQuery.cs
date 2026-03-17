using MediatR;
using SpotFinder.AuthService.Business.Models;
using SpotFinder.AuthService.Data.Repositories;

namespace SpotFinder.AuthService.Business.Queries;

public record GetFavoritesQuery(Guid UserId) : IRequest<IEnumerable<FavoriteDto>>;

public class GetFavoritesQueryHandler : IRequestHandler<GetFavoritesQuery, IEnumerable<FavoriteDto>>
{
    private readonly IUserInteractionRepository _repo;

    public GetFavoritesQueryHandler(IUserInteractionRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<FavoriteDto>> Handle(GetFavoritesQuery request, CancellationToken ct)
    {
        var favorites = await _repo.GetFavoritesAsync(request.UserId, ct);
        return favorites.Select(f => new FavoriteDto(f.VenueId, f.CreatedAt));
    }
}
