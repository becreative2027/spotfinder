using MediatR;
using SpotFinder.AuthService.Business.Models;
using SpotFinder.AuthService.Data.Repositories;

namespace SpotFinder.AuthService.Business.Queries;

public record GetVisitsQuery(Guid UserId) : IRequest<IEnumerable<VisitDto>>;

public class GetVisitsQueryHandler : IRequestHandler<GetVisitsQuery, IEnumerable<VisitDto>>
{
    private readonly IUserInteractionRepository _repo;

    public GetVisitsQueryHandler(IUserInteractionRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<VisitDto>> Handle(GetVisitsQuery request, CancellationToken ct)
    {
        var visits = await _repo.GetVisitsAsync(request.UserId, ct);
        return visits.Select(v => new VisitDto(v.Id, v.VenueId, v.VisitedAt));
    }
}
