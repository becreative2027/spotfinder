namespace SpotFinder.AuthService.Data.Entities;

public class UserFavorite
{
    public Guid UserId { get; set; }
    public Guid VenueId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
