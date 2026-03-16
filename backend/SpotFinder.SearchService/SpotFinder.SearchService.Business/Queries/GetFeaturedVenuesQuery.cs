using MediatR;
using SpotFinder.SearchService.Business.Models;
using SpotFinder.SearchService.Data.Entities;
using SpotFinder.SearchService.Data.Repositories;

namespace SpotFinder.SearchService.Business.Queries;

public record GetFeaturedVenuesQuery(int Count = 10) : IRequest<IEnumerable<VenueSummaryDto>>;

public class GetFeaturedVenuesQueryHandler : IRequestHandler<GetFeaturedVenuesQuery, IEnumerable<VenueSummaryDto>>
{
    private readonly ISearchRepository _repo;

    public GetFeaturedVenuesQueryHandler(ISearchRepository repo) => _repo = repo;

    public async Task<IEnumerable<VenueSummaryDto>> Handle(GetFeaturedVenuesQuery request, CancellationToken ct)
    {
        var count = Math.Clamp(request.Count, 1, 50);
        var items = await _repo.GetFeaturedAsync(count, ct);
        return items.Select(MapToDto);
    }

    private static VenueSummaryDto MapToDto(VenueReadModel v) => new(
        v.Id,
        v.Name,
        v.Description,
        v.DistrictId,
        v.District?.Name,
        v.Address,
        v.ParkingStatus,
        v.Lat,
        v.Lng,
        v.AverageRating,
        v.ReviewCount,
        v.CreatedAt,
        v.Photos.OrderBy(p => p.DisplayOrder).Select(p => new VenuePhotoSummaryDto(p.Id, p.Url, p.IsMenuPhoto, p.DisplayOrder)),
        v.VenueConcepts.Select(vc => new ConceptTagSummaryDto(vc.ConceptTag.Id, vc.ConceptTag.NameTr, vc.ConceptTag.NameEn)));
}
