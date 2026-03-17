namespace SpotFinder.AuthService.Business.Models;

public record FavoriteDto(Guid VenueId, DateTime CreatedAt);
public record VisitDto(Guid Id, Guid VenueId, DateTime VisitedAt);
