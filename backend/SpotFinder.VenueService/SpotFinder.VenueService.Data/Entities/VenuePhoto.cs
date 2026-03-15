namespace SpotFinder.VenueService.Data.Entities;

public class VenuePhoto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VenueId { get; set; }
    public string Url { get; set; } = null!;
    public bool IsMenuPhoto { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Venue Venue { get; set; } = null!;
}
