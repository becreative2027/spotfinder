namespace SpotFinder.ReviewService.Data.Entities;

public class Review
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }
    public Guid UserId { get; set; }
    public string? Body { get; set; }
    public short Rating { get; set; }
    public string Status { get; set; } = "pending"; // pending | approved | rejected
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
