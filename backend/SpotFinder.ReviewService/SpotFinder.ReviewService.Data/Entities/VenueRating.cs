namespace SpotFinder.ReviewService.Data.Entities;

/// <summary>
/// Minimal write model for updating venue rating/count in venue.venues table.
/// ReviewService updates this directly since both services share the same DB.
/// </summary>
public class VenueRating
{
    public Guid Id { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
}
