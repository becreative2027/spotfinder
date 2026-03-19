using FluentValidation;
using MediatR;
using SpotFinder.SearchService.Business.Models;
using SpotFinder.SearchService.Data.Entities;
using SpotFinder.SearchService.Data.Repositories;

namespace SpotFinder.SearchService.Business.Queries;

public record SearchVenuesQuery(
    int? DistrictId,
    IEnumerable<int>? ConceptTagIds,
    string SortBy = "rating",
    int Page = 1,
    int PageSize = 20,
    string? NameQuery = null) : IRequest<PagedResult<VenueSummaryDto>>;

public class SearchVenuesQueryHandler : IRequestHandler<SearchVenuesQuery, PagedResult<VenueSummaryDto>>
{
    private readonly ISearchRepository _repo;

    public SearchVenuesQueryHandler(ISearchRepository repo) => _repo = repo;

    public async Task<PagedResult<VenueSummaryDto>> Handle(SearchVenuesQuery request, CancellationToken ct)
    {
        var sortBy = request.SortBy?.ToLowerInvariant() == "newest" ? "newest" : "rating";

        var (items, totalCount) = await _repo.SearchAsync(
            request.DistrictId,
            request.ConceptTagIds,
            sortBy,
            request.Page,
            request.PageSize,
            request.NameQuery,
            ct);

        var dtos = items.Select(MapToDto);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new PagedResult<VenueSummaryDto>(dtos, totalCount, request.Page, request.PageSize, totalPages);
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

public class SearchVenuesQueryValidator : AbstractValidator<SearchVenuesQuery>
{
    public SearchVenuesQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
        RuleFor(x => x.SortBy)
            .Must(s => s == null || s == "rating" || s == "newest")
            .WithMessage("sortBy must be 'rating' or 'newest'.");
    }
}
