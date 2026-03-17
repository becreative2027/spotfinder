namespace SpotFinder.ReviewService.Business.Models;

public record ReviewDto(
    Guid Id,
    Guid VenueId,
    Guid UserId,
    string? Body,
    short Rating,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
