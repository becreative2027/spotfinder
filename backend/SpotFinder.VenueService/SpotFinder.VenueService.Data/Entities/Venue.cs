namespace SpotFinder.VenueService.Data.Entities;

public class Venue
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int? DistrictId { get; set; }
    public string? Address { get; set; }
    public string? ParkingStatus { get; set; } // available | unavailable | valet
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public decimal AverageRating { get; set; } = 0;
    public int ReviewCount { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public District? District { get; set; }
    public ICollection<VenuePhoto> Photos { get; set; } = new List<VenuePhoto>();
    public ICollection<VenueConcept> VenueConcepts { get; set; } = new List<VenueConcept>();
}
