namespace SpotFinder.AuthService.Data.Entities;

public class UserVisit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid VenueId { get; set; }
    public DateTime VisitedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
