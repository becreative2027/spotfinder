namespace SpotFinder.SearchService.Business.Models;

public record VenueSummaryDto(
    Guid Id,
    string Name,
    string? Description,
    int? DistrictId,
    string? DistrictName,
    string? Address,
    string? ParkingStatus,
    decimal? Lat,
    decimal? Lng,
    decimal AverageRating,
    int ReviewCount,
    DateTime CreatedAt,
    IEnumerable<VenuePhotoSummaryDto> Photos,
    IEnumerable<ConceptTagSummaryDto> ConceptTags);

public record VenuePhotoSummaryDto(Guid Id, string Url, bool IsMenuPhoto, int DisplayOrder);

public record ConceptTagSummaryDto(int Id, string NameTr, string NameEn);

public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize, int TotalPages);
