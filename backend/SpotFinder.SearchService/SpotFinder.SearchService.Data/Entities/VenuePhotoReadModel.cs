namespace SpotFinder.SearchService.Data.Entities;

public class VenuePhotoReadModel
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }
    public string Url { get; set; } = null!;
    public bool IsMenuPhoto { get; set; }
    public int DisplayOrder { get; set; }
}
