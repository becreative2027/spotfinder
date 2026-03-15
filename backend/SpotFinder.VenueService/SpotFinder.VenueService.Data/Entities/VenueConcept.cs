namespace SpotFinder.VenueService.Data.Entities;

public class VenueConcept
{
    public Guid VenueId { get; set; }
    public int ConceptTagId { get; set; }

    public Venue Venue { get; set; } = null!;
    public ConceptTag ConceptTag { get; set; } = null!;
}
