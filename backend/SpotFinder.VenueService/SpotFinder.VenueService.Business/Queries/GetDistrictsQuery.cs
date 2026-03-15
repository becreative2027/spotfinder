using MediatR;
using SpotFinder.VenueService.Business.Models;
using SpotFinder.VenueService.Data.Repositories;

namespace SpotFinder.VenueService.Business.Queries;

public record GetDistrictsQuery : IRequest<IEnumerable<DistrictDto>>;

public class GetDistrictsQueryHandler : IRequestHandler<GetDistrictsQuery, IEnumerable<DistrictDto>>
{
    private readonly IDistrictRepository _repo;
    public GetDistrictsQueryHandler(IDistrictRepository repo) => _repo = repo;

    public async Task<IEnumerable<DistrictDto>> Handle(GetDistrictsQuery request, CancellationToken ct)
    {
        var districts = await _repo.GetAllAsync(ct);
        return districts.Select(d => new DistrictDto(d.Id, d.Name, d.City));
    }
}
