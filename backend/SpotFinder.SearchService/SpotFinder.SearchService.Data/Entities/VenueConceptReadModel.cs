namespace SpotFinder.SearchService.Data.Entities;

public class VenueConceptReadModel
{
    public Guid VenueId { get; set; }
    public int ConceptTagId { get; set; }
    public ConceptTagReadModel ConceptTag { get; set; } = null!;
}
