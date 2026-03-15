using FluentValidation;
using MediatR;
using SpotFinder.VenueService.Business.Models;
using SpotFinder.VenueService.Data.Repositories;

namespace SpotFinder.VenueService.Business.Queries;

public record GetVenuesQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResult<VenueDto>>;

public class GetVenuesQueryHandler : IRequestHandler<GetVenuesQuery, PagedResult<VenueDto>>
{
    private readonly IVenueRepository _repo;
    public GetVenuesQueryHandler(IVenueRepository repo) => _repo = repo;

    public async Task<PagedResult<VenueDto>> Handle(GetVenuesQuery request, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(request.Page, request.PageSize, ct);
        var dtos = items.Select(v => new VenueDto(
            v.Id, v.Name, v.Description,
            v.DistrictId, v.District?.Name,
            v.Address, v.ParkingStatus,
            v.Lat, v.Lng,
            v.AverageRating, v.ReviewCount, v.IsActive, v.CreatedAt,
            v.Photos.Select(p => new VenuePhotoDto(p.Id, p.Url, p.IsMenuPhoto, p.DisplayOrder)),
            v.VenueConcepts.Select(vc => new ConceptTagDto(vc.ConceptTag.Id, vc.ConceptTag.NameTr, vc.ConceptTag.NameEn, vc.ConceptTag.IsSystem, vc.ConceptTag.IsActive))
        ));
        int totalPages = (int)Math.Ceiling(total / (double)request.PageSize);
        return new PagedResult<VenueDto>(dtos, total, request.Page, request.PageSize, totalPages);
    }
}

public class GetVenuesQueryValidator : AbstractValidator<GetVenuesQuery>
{
    public GetVenuesQueryValidator()
    {
        RuleFor(q => q.Page).GreaterThan(0);
        RuleFor(q => q.PageSize).InclusiveBetween(1, 50);
    }
}
