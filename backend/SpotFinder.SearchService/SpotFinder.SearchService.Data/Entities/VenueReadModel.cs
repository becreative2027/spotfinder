namespace SpotFinder.SearchService.Data.Entities;

public class VenueReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int? DistrictId { get; set; }
    public string? Address { get; set; }
    public string? ParkingStatus { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public DistrictReadModel? District { get; set; }
    public ICollection<VenuePhotoReadModel> Photos { get; set; } = new List<VenuePhotoReadModel>();
    public ICollection<VenueConceptReadModel> VenueConcepts { get; set; } = new List<VenueConceptReadModel>();
}
