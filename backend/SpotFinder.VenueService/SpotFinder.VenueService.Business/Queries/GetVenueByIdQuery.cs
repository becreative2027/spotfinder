using MediatR;
using SpotFinder.VenueService.Business.Models;
using SpotFinder.VenueService.Data.Repositories;

namespace SpotFinder.VenueService.Business.Queries;

public record GetVenueByIdQuery(Guid Id) : IRequest<VenueDto?>;

public class GetVenueByIdQueryHandler : IRequestHandler<GetVenueByIdQuery, VenueDto?>
{
    private readonly IVenueRepository _repo;
    public GetVenueByIdQueryHandler(IVenueRepository repo) => _repo = repo;

    public async Task<VenueDto?> Handle(GetVenueByIdQuery request, CancellationToken ct)
    {
        var v = await _repo.GetByIdAsync(request.Id, ct);
        if (v is null) return null;
        return new VenueDto(
            v.Id, v.Name, v.Description,
            v.DistrictId, v.District?.Name,
            v.Address, v.ParkingStatus,
            v.Lat, v.Lng,
            v.AverageRating, v.ReviewCount, v.IsActive, v.CreatedAt,
            v.Photos.Select(p => new VenuePhotoDto(p.Id, p.Url, p.IsMenuPhoto, p.DisplayOrder)),
            v.VenueConcepts.Select(vc => new ConceptTagDto(vc.ConceptTag.Id, vc.ConceptTag.NameTr, vc.ConceptTag.NameEn, vc.ConceptTag.IsSystem, vc.ConceptTag.IsActive))
        );
    }
}
