namespace SpotFinder.VenueService.Business.Models;

public record ConceptTagDto(
    int Id,
    string NameTr,
    string NameEn,
    bool IsSystem,
    bool IsActive
);
