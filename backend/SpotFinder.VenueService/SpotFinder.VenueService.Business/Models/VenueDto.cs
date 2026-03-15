namespace SpotFinder.VenueService.Business.Models;

public record VenueDto(
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
    bool IsActive,
    DateTime CreatedAt,
    IEnumerable<VenuePhotoDto> Photos,
    IEnumerable<ConceptTagDto> ConceptTags
);

public record VenuePhotoDto(
    Guid Id,
    string Url,
    bool IsMenuPhoto,
    int DisplayOrder
);

public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
