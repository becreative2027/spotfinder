namespace SpotFinder.VenueService.Data.Entities;

public class ConceptTag
{
    public int Id { get; set; }
    public string NameTr { get; set; } = null!;
    public string NameEn { get; set; } = null!;
    public bool IsSystem { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<VenueConcept> VenueConcepts { get; set; } = new List<VenueConcept>();
}
